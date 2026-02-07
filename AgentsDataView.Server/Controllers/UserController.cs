using AgentsDataView.Common;
using AgentsDataView.Common.Exceptions;
using AgentsDataView.Common.Utilities;
using AgentsDataView.Data.Contracts;
using AgentsDataView.Data.Repositories;
using AgentsDataView.Entities;
using AgentsDataView.Entities.DtoModels;
using AgentsDataView.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Principal;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IranSAS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IJwtService jwtService,
                                IUserRepository userRepository,
                                IRepository<Company> companyRepo,
                                IRepository<RefreshToken> refreshTokenRepo,
                                IRepository<CompanyUserRelation> compUserRelationRepo
                                ) : ControllerBase
    {
        private readonly IJwtService _jwtService = jwtService;
        private readonly IRepository<Company> _companyRepo = companyRepo;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRepository<RefreshToken> _refreshTokenRepo = refreshTokenRepo;
        private readonly IRepository<CompanyUserRelation> _compUserRelationRepo = compUserRelationRepo;


        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult<AccessToken>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {

            var user = await _userRepository.FindByUserAndPass(request.Username, request.Password, cancellationToken);
            if (user == null)
            {
                return BadRequest("نام کاربری یا رمز عبور اشتباه است");
            }
            TokenBundle jwt = _jwtService.Generate(user);
            var refreshToken = new RefreshToken()
            {
                Id = 0,
                CreationDate = jwt.RefreshToken.CreationDate,
                ExpireDate = jwt.RefreshToken.ExpireDate,
                IsRevoked = jwt.RefreshToken.IsRevoked,
                ReplaceByToken = jwt.RefreshToken.ReplaceByToken,
                UserId = jwt.RefreshToken.UserId,
                Token = jwt.RefreshToken.Token,
            };
            await _refreshTokenRepo.AddAsync(refreshToken, cancellationToken).ConfigureAwait(false);
            //نیاز به منتظر ماندن برای دریافت نتیجه بروزرسانی تاریخ ورود به سیستم نیست 
            await _userRepository.UpdateLastLoginDateAsync(user, cancellationToken);
            return Ok(jwt.AccessToken);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<UserDto[]>> GetAll(int? companyId, CancellationToken cancellationToken)
        {
            var qry = _userRepository.QueryNoTracking.Where(q => !q.IsApiUser && q.UserName != "super");
            if (companyId > 0)
            {
                qry = qry.Where(q => q.CompanyUserRelations.Any(c => c.CompanyId == companyId));
            }
            UserDto[] result = await qry.Select(u =>
            new UserDto()
            {
                UserFullName = u.UserFullName,
                Id = u.Id,
                IsActive = u.IsActive,
                UserMobile = u.UserMobile,
                UserName = u.UserName,
                CompanyIds = u.CompanyUserRelations.Select(c => c.CompanyId).ToArray(),
                ProvinceIds = u.CompanyUserRelations.Select(c=>c.Company.ProvinceId.Value).Distinct().ToArray(),
            })
                .ToArrayAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SystemUser>> Get(int id, CancellationToken cancellationToken)
        {
            SystemUser? user = await _userRepository.GetByIdAsync(cancellationToken, id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<SystemUser>> Create(UserDto model, CancellationToken cancellationToken)
        {
            if (!model.Password.HasValue())
            {
                model.Password = model.UserMobile;
            }
            SystemUser user = new()
            {
                Id = model.Id,
                UserMobile = model.UserMobile,
                Password = model.Password,
                UserFullName = model.UserFullName,
                UserName = model.UserName ?? ""
            };
            await using var transaction = await _userRepository.BeginTransaction(cancellationToken);
            try
            {

                await _userRepository.AddAsync(user, cancellationToken);

                #region ===================================== Relations ======================

                if (model.ProvinceIds?.Length > 0 && !(model.CompanyIds?.Length > 0))
                {
                    model.ProvinceIds = model.ProvinceIds.Distinct().ToArray();
                    model.CompanyIds =
                    await _companyRepo.QueryNoTracking.Where(c => c.ProvinceId != null && model.ProvinceIds.Contains(c.ProvinceId.Value))
                         .Select(c => c.Id).ToArrayAsync(cancellationToken);
                }
                if (model.CompanyIds?.Length > 0)
                {
                    var relations = model.CompanyIds.Distinct().Select(c => new CompanyUserRelation() { Id = 0, CompanyId = c, UserId = user.Id });
                    await _compUserRelationRepo.AddRangeAsync(relations, cancellationToken);

                }
                else
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return BadRequest("هیچ شرکتی در استان های انتخاب شده ثبت نشده است.");
                }

                #endregion

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
            user.Password = null;
            return Ok(user);
        }

        [HttpPut("[action]")]
        public async Task<ActionResult> Update([FromBody] UserDto model, CancellationToken cancellationToken)
        {
            SystemUser? user = await _userRepository.GetByIdAsync(cancellationToken, model.Id);
            if (user == null)
            {
                return NotFound();
            }
            user.UserFullName = model.UserFullName;
            user.UserName = model.UserName;
            user.UserMobile = model.UserMobile;
            await using var transaction = await _userRepository.BeginTransaction(cancellationToken);
            try
            {

                await _userRepository.UpdateAsync(user, cancellationToken);
                #region ===================================== Relations ======================

                if (model.ProvinceIds?.Length > 0 && !(model.CompanyIds?.Length > 0))
                {
                    model.ProvinceIds = model.ProvinceIds.Distinct().ToArray();
                    model.CompanyIds =
                    await _companyRepo.QueryNoTracking.Where(c => c.ProvinceId != null && model.ProvinceIds.Contains(c.ProvinceId.Value))
                         .Select(c => c.Id).ToArrayAsync(cancellationToken);
                }
                if (model.CompanyIds?.Length > 0)
                {
                    var relations = model.CompanyIds.Distinct().Select(c => new CompanyUserRelation() { Id = 0, CompanyId = c, UserId = user.Id });
                    var existingRelations =  _compUserRelationRepo.Query.Where(c => c.UserId == user.Id);
                    await _compUserRelationRepo.DeleteRangeAsync(existingRelations, cancellationToken, false);
                    await _compUserRelationRepo.AddRangeAsync(relations, cancellationToken);

                }

                #endregion
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {

               await transaction.RollbackAsync(cancellationToken);
                throw;
            }
            return Ok();
        }
        [HttpPut("[action]")]
        public async Task<ActionResult> UpdateMe([FromBody] UpdateMeDto model, CancellationToken cancellationToken)
        {
            IIdentity? identity = HttpContext.User.Identity;
            int userId = identity.GetUserId<int>();
            SystemUser? user = await _userRepository.GetByIdAsync(cancellationToken, userId);
            if (user == null)
            {
                return NotFound();
            }
            user.UserFullName = model.UserFullName;
            user.UserName = model.UserName;
            user.UserMobile = model.UserMobile;
            if (!string.IsNullOrWhiteSpace(model.Password) && model.Password != "____________________")
            {
                string passwordHash = SecurityHelper.GetSha256Hash(model.Password);
                user.Password = passwordHash;
            }
            await _userRepository.UpdateAsync(user, cancellationToken);
            return Ok();
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _userRepository.DeleteAsync(id, cancellationToken);
            return Ok("Success");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<AccessToken>> RefreshToken([FromBody] RefreshRequest model, CancellationToken cancellationToken)
        {
            var refreshToken = _refreshTokenRepo.Query
                .FirstOrDefault(x => x.Token == model.RefreshToken && !x.IsRevoked);

            if (refreshToken == null || refreshToken.ExpireDate < DateTime.UtcNow)
                return Unauthorized(MessagesDictionary.Messages["invalid refresh token"]);

            var user = await _userRepository.QueryNoTracking.Include(u => u.CompanyUserRelations).FirstOrDefaultAsync(u => u.Id == refreshToken.UserId, cancellationToken);
            if (user == null)
            {
                return Unauthorized(MessagesDictionary.Messages["invalid refresh token"]);
            }

            // ساخت AccessToken جدید
            var tokenBundle = _jwtService.Generate(user);

            // ابطال رفرش توکن قبلی
            refreshToken.IsRevoked = true;
            refreshToken.ReplaceByToken = tokenBundle.RefreshToken.Token;
            await _refreshTokenRepo.UpdateAsync(refreshToken, cancellationToken, false).ConfigureAwait(false);
            await _refreshTokenRepo.AddAsync(tokenBundle.RefreshToken, cancellationToken).ConfigureAwait(false);
            return Ok(tokenBundle.AccessToken);
        }

        public class RefreshRequest
        {
            public string RefreshToken { get; set; } = "";
        }

    }
}

using AgentsDataView.Common;
using AgentsDataView.Common.Exceptions;
using AgentsDataView.Common.Utilities;
using AgentsDataView.Data.Contracts;
using AgentsDataView.Data.Repositories;
using AgentsDataView.Entities;
using AgentsDataView.Entities.DtoModels;
using AgentsDataView.Services;
using AgentsDataView.WebFramework.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IranSAS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IRepository<RefreshToken> refreshTokenRepo, IUserRepository userRepository, IJwtService jwtService) : ControllerBase
    {
        private readonly IRepository<RefreshToken> _refreshTokenRepo = refreshTokenRepo;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IJwtService _jwtService = jwtService;

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
            var qry = _userRepository.QueryNoTracking.Where(q => q.UserName != "super");
            if (companyId > 0)
            {
                qry = qry.Where(q => q.CompanyId == companyId);
            }
            UserDto[] result = await qry.Select(u =>
            new UserDto()
            {
                UserFullName = u.UserFullName,
                Id = u.Id,
                IsActive = u.IsActive,
                UserMobile = u.UserMobile,
                UserName = u.UserName
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
                CompanyId = model.CompanyId,
                Id = model.Id,
                UserMobile = model.UserMobile,
                Password = model.Password,
                UserFullName = model.UserFullName,
                UserName = model.UserName
            };
            await _userRepository.AddAsync(user, cancellationToken);
            user.Password = null;
            return Ok( user);
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
            await _userRepository.UpdateAsync(user, cancellationToken);
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

            var user = await _userRepository.GetByIdAsync(cancellationToken, refreshToken.UserId);
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

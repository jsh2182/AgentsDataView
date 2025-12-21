using AgentsDataView.Common.Utilities;
using AgentsDataView.Data.Contracts;
using AgentsDataView.Data.Repositories;
using AgentsDataView.Entities;
using AgentsDataView.Entities.DtoModels;
using AgentsDataView.Services;
using AgentsDataView.WebFramework.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgentsDataView.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController(IRepository<Company> companyRepo, IRepository<FinancialPeriod> fpRepo, IInvoiceService invoiceService, IUserRepository userRepo) : ControllerBase
    {
        private readonly IRepository<Company> _companyRepo = companyRepo;
        private readonly IRepository<FinancialPeriod> _fpRepo = fpRepo;
        private readonly IUserRepository _userRepo = userRepo;
        private readonly IInvoiceService _invoiceService = invoiceService;

        [HttpGet]
        public async Task<ActionResult<CompaniesDto>> Get(int id, CancellationToken cancellationToken)
        {
            if (id < 1)
            {
                return BadRequest("شناسه ارسالی معتبر نیست.");
            }
            var comp = await _companyRepo.QueryNoTracking.Include(c => c.Province).Include(c => c.City).FirstOrDefaultAsync(c => c.Id == id, cancellationToken).ConfigureAwait(false);
            if (comp == null)
            {
                return BadRequest("اطلاعات درخواستی در سیستم وجود ندارد.");
            }
            var result = new CompaniesDto()
            {
                Address = comp.Address,
                CityId = comp.CityId,
                CityName = comp.City?.CityName,
                Code = comp.Code,
                Id = comp.Id,
                Name = comp.Name,
                PhoneNumber = comp.PhoneNumber,
                PostalCode = comp.PostalCode,
                ProvinceId = comp.ProvinceId,
                ProvinceName = comp.Province?.ProvinceName,
                RegistrationNumber = comp.RegistrationNumber,
            };
            return Ok(result);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<long>> GetIdByCode(string code, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("کد ارسالی معتبر نیست.");
            }
            var compId = await _companyRepo.QueryNoTracking.Where(c => c.Code == code).Select(c => c.Id).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            if (compId < 1)
            {
                return BadRequest("اطلاعات درخواستی در سیستم وجود ندارد.");
            }
            return Ok(compId);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<CompaniesDto[]>> GetAll(CancellationToken cancellationToken)
        {
            var array = await _companyRepo.QueryNoTracking
                .Include(c => c.Province)
                .Include(c => c.City)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);
            var result = array.Select(comp => new CompaniesDto()
            {
                Address = comp.Address,
                CityId = comp.CityId,
                CityName = comp.City?.CityName,
                Code = comp.Code,
                Id = comp.Id,
                Name = comp.Name,
                PhoneNumber = comp.PhoneNumber,
                PostalCode = comp.PostalCode,
                ProvinceId = comp.ProvinceId,
                ProvinceName = comp.Province?.ProvinceName,
                RegistrationNumber = comp.RegistrationNumber,
            }).ToArray();

            var invDict = await _invoiceService.GetMaxInvoiceDates(cancellationToken);
            foreach(var c in result)
            {
                invDict.TryGetValue(c.Id.Value, out var inv);
                c.MaxInvoiceCreationDate = inv.MaxCreationDate;
                c.MaxInvoiceDate = inv.MaxInvoiceDate;
            }

            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<CreateCompanyResult>> Create(CompaniesDto model, CancellationToken cancellationToken)
        {
            string[] stringArray = [model.Name ,
            model.Address ,
            model.Code,
            model.PhoneNumber,
            model.PostalCode ,
            model.RegistrationNumber ];
            stringArray.CleanStrings();

            var existingComp = await _companyRepo.QueryNoTracking.FirstOrDefaultAsync(c =>
            c.Name == stringArray[0] ||
            stringArray[1] == c.Code ||
            (!string.IsNullOrEmpty(stringArray[2]) && c.Address == stringArray[2]) ||
            (!string.IsNullOrEmpty(stringArray[3]) && stringArray[3] == c.PostalCode) ||
            (!string.IsNullOrEmpty(stringArray[4]) && stringArray[4] == c.PhoneNumber) ||
            (!string.IsNullOrEmpty(stringArray[5]) && stringArray[5] == c.RegistrationNumber),
            cancellationToken
            ).ConfigureAwait(false);
            if (existingComp != null)
            {
                if (stringArray[0] == existingComp.Name)
                {
                    return BadRequest("شرکت دیگری با این نام ثبت شده است.");
                }
                if (stringArray[1] == existingComp.Code)
                {
                    return BadRequest("شرکت دیگری با این کد ثبت شده است.");
                }
                if (stringArray[2] == existingComp.Address)
                {
                    return BadRequest("شرکت دیگری با این نشانی ثبت شده است.");
                }
                if (stringArray[3] == existingComp.PostalCode)
                {
                    return BadRequest("شرکت دیگری با این کد پستی ثبت شده است.");
                }
                if (stringArray[4] == existingComp.PhoneNumber)
                {
                    return BadRequest("شرکت دیگری با این شماره تلفن ثبت شده است."); ;
                }
                if (stringArray[5] == existingComp.RegistrationNumber)
                {
                    return BadRequest("شرکت دیگری با این کد پستی ثبت شده است.");
                }

            }
            Company newComp = new()
            {
                Id = 0,
                Name = model.Name,
                Address = model.Address,
                PostalCode = model.PostalCode,
                PhoneNumber = model.PhoneNumber,
                CityId = model.CityId,
                Code = model.Code,
                ProvinceId = model.ProvinceId,
                RegistrationNumber = model.RegistrationNumber
            };
            await using var transaction = await _companyRepo.BeginTransaction(cancellationToken);

            try
            {
                await _companyRepo.AddAsync(newComp, cancellationToken, true);

                // 2️⃣ افزودن دوره مالی بدون SaveChangesAsync (saveNow = false)
                var newFp = new FinancialPeriod
                {
                    Id=0,
                    CompanyId = newComp.Id,
                    Title = $"دوره مالی شرکت {newComp.Name}",
                    StartDate = DateTimeOffset.UtcNow
                };
                await _fpRepo.AddAsync(newFp, cancellationToken, false);

                // 3️⃣ افزودن کاربر شرکت و SaveChangesAsync همان لحظه (saveNow = true)
                var compUser = new SystemUser
                {
                    Id=0,
                    CompanyId = newComp.Id,
                    IsActive = true,
                    Password = "Comp_" + newComp.Id,
                    UserFullName = $"کاربر شرکت {newComp.Name}",
                    UserName = "Comp_" + newComp.Id
                };
                await _userRepo.AddAsync(compUser, cancellationToken); // اینجا SaveChangesAsync زده میشه

                // ✅ commit تراکنش
                await transaction.CommitAsync(cancellationToken);

                return Ok(new CreateCompanyResult
                {
                    CompanyId = newComp.Id,
                    FinancialPeriodId = newFp.Id,
                    UserName = compUser.UserName
                });
            }
            catch
            {
                // rollback اگر خطایی رخ بده
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<string>> Update(CompaniesDto model, CancellationToken cancellationToken)
        {
            if (!model.Id.HasValue || model.Id < 1)
            {
                return BadRequest("شناسه ارسالی معتبر نیست.");
            }
            Company? comp = await _companyRepo.GetByIdAsync(cancellationToken, model.Id).ConfigureAwait(false);
            if (comp == null)
            {
                return BadRequest("اطلاعات درخواستی در سیستم وجود ندارد.");
            }
            string[] stringArray = [model.Name ,
            model.Address ,
            model.Code,
            model.PhoneNumber,
            model.PostalCode ,
            model.RegistrationNumber ];
            stringArray.CleanStrings();

            var existingComp = await _companyRepo.QueryNoTracking.FirstOrDefaultAsync(c =>
            c.Id != model.Id && (
            c.Name == stringArray[0] ||
            stringArray[1] == c.Code ||
            (!string.IsNullOrEmpty(stringArray[2]) && c.Address == stringArray[2]) ||
            (!string.IsNullOrEmpty(stringArray[3]) && stringArray[3] == c.PostalCode) ||
            (!string.IsNullOrEmpty(stringArray[4]) && stringArray[4] == c.PhoneNumber) ||
            (!string.IsNullOrEmpty(stringArray[5]) && stringArray[5] == c.RegistrationNumber)),
            cancellationToken
            ).ConfigureAwait(false);
            if (existingComp != null)
            {
                if (stringArray[0] == existingComp.Name)
                {
                    return BadRequest("شرکت دیگری با این نام ثبت شده است.");
                }
                if (stringArray[1] == existingComp.Code)
                {
                    return BadRequest("شرکت دیگری با این کد ثبت شده است.");
                }
                if (stringArray[2] == existingComp.Address)
                {
                    return BadRequest("شرکت دیگری با این نشانی ثبت شده است.");
                }
                if (stringArray[3] == existingComp.PostalCode)
                {
                    return BadRequest("شرکت دیگری با این کد پستی ثبت شده است.");
                }
                if (stringArray[4] == existingComp.PhoneNumber)
                {
                    return BadRequest("شرکت دیگری با این شماره تلفن ثبت شده است."); ;
                }
                if (stringArray[5] == existingComp.RegistrationNumber)
                {
                    return BadRequest("شرکت دیگری با این کد پستی ثبت شده است.");
                }

            }


            comp.Name = model.Name;
            comp.Address = model.Address;
            comp.PostalCode = model.PostalCode;
            comp.PhoneNumber = model.PhoneNumber;
            comp.CityId = model.CityId;
            comp.Code = model.Code;
            comp.ProvinceId = model.ProvinceId;
            comp.RegistrationNumber = model.RegistrationNumber;

            await _companyRepo.UpdateAsync(comp, cancellationToken).ConfigureAwait(false);
            return Ok("Success");
        }

    }
}

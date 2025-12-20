using AgentsDataView.Common;
using AgentsDataView.Common.Utilities;
using AgentsDataView.Data.Contracts;
using AgentsDataView.Entities;
using AgentsDataView.Entities.DtoModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgentsDataView.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialPeriodsController(IRepository<FinancialPeriod> financialPeriodRepo) : ControllerBase
    {
        private readonly IRepository<FinancialPeriod> _financialPeriodRepo = financialPeriodRepo;

        [HttpGet]
        public async Task<ActionResult<FinancialPeriodDto>> Get(int id, CancellationToken cancellationToken)
        {
            if (id < 1)
            {
                return BadRequest("شناسه ارسالی معتبر نیست.");
            }
            var fp = await _financialPeriodRepo.GetByIdAsync(cancellationToken, id).ConfigureAwait(false);
            var result = new FinancialPeriodDto()
            {
                EndDate = fp.EndDate,
                StartDate = fp.StartDate,
                Title = fp.Title
            };
            return Ok(result);
        }
        [HttpPost("[action]")]
        public async Task<ActionResult<int>> Create(FinancialPeriodDto dto, CancellationToken cancellationToken)
        {
            var identity = HttpContext.User.Identity;
            int companyId = identity.GetCompanyId();
            dto.Title = dto.Title.CleanString2();
             bool isDup = await _financialPeriodRepo.QueryNoTracking.AnyAsync(q=>q.Title == dto.Title && q.CompanyId == companyId, cancellationToken ).ConfigureAwait(false);
            if (isDup)
            {
                return BadRequest("دوره دیگری با این نام در سیستم وجود دارد.");
            }
            var overlappedBy = await _financialPeriodRepo.QueryNoTracking.FirstOrDefaultAsync(q => 
            ((dto.StartDate >= q.StartDate && dto.StartDate <= q.EndDate)
                  || (dto.EndDate >= q.StartDate && dto.EndDate <= q.EndDate)) && q.CompanyId == companyId, cancellationToken).ConfigureAwait(false);
            if (overlappedBy != null)
            {
                return BadRequest($"این دوره مالی با دوره مالی ${overlappedBy.Title} همپوشانی دارد.");
            }
            var fp = new FinancialPeriod()
            {
                Id = 0,
                CompanyId = companyId,
                EndDate = dto.EndDate,
                StartDate = dto.StartDate,
                Title = dto.Title
            };
            await _financialPeriodRepo.AddAsync(fp, cancellationToken).ConfigureAwait(false);
            return Ok(fp.Id);
        }
        [HttpPut("[action]")]
        public async Task<ActionResult<int>> Update(FinancialPeriodDto dto, CancellationToken cancellationToken)
        {
            var identity = HttpContext.User.Identity;
            int companyId = identity.GetCompanyId();
            var fp = await _financialPeriodRepo.Query.FirstOrDefaultAsync(f=>f.Id == dto.Id && f.CompanyId == companyId, cancellationToken).ConfigureAwait(false);
            if (fp == null)
            {
                return BadRequest("اطلاعات درخواستی در سیستم وجود ندارد");
            }
            dto.Title = dto.Title.CleanString2();
            bool isDup = await _financialPeriodRepo.QueryNoTracking.AnyAsync(q => q.Title == dto.Title && q.CompanyId == companyId && q.Id != dto.Id, cancellationToken).ConfigureAwait(false);
            if (isDup)
            {
                return BadRequest("دوره دیگری با این نام در سیستم وجود دارد.");
            }
            var overlappedBy = await _financialPeriodRepo.QueryNoTracking.FirstOrDefaultAsync(q =>
            q.Id != dto.Id &&
            ((dto.StartDate >= q.StartDate && dto.StartDate <= q.EndDate)
                  || (dto.EndDate >= q.StartDate && dto.EndDate <= q.EndDate)) && q.CompanyId == companyId, cancellationToken).ConfigureAwait(false);
            if (overlappedBy != null)
            {
                return BadRequest($"این دوره مالی با دوره مالی ${overlappedBy.Title} همپوشانی دارد.");
            }

            fp.EndDate = dto.EndDate;
            fp.StartDate = dto.StartDate;
            fp.Title = dto.Title;

            await _financialPeriodRepo.UpdateAsync(fp, cancellationToken).ConfigureAwait(false);
            return Ok(fp.Id);
        }
    }

}

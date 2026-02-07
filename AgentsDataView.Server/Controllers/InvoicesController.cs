using AgentsDataView.Common;
using AgentsDataView.Common.Utilities;
using AgentsDataView.Data.Contracts;
using AgentsDataView.Entities;
using AgentsDataView.Entities.DtoModels;
using AgentsDataView.Server.Services;
using AgentsDataView.Services;
using AgentsDataView.WebFramework.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Globalization;
using System.Security.Principal;

namespace AgentsDataView.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController(IInvoiceService invoiceService, IIsUpdatingInvoices isUpdatingInvoices, IRepository<Setting> settingRepo) : ControllerBase
    {
        private readonly IInvoiceService _invoiceService = invoiceService;
        private readonly IIsUpdatingInvoices _isUpdatingInvoices = isUpdatingInvoices;
        private readonly IRepository<Setting> _settingRepo = settingRepo;

        [HttpGet]
        public async Task<ActionResult<InvoiceDto>> Get(long invoiceId, CancellationToken cancellationToken)
        {
            int? companyId = HttpContext.User.Identity?.GetCompanyId();
            if (!(companyId > 0))
            {
                return BadRequest("شرکت درخواست دهنده معتبر نیست.");
            }
            InvoiceDto result = await _invoiceService.Find(invoiceId, companyId.Value, cancellationToken);
            return Ok(result);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<InvoiceDto>> GetByNumber(int invoiceNumber, BaseInfoes.InvoiceTypes invoiceType, int financialPeriodId, CancellationToken cancellationToken)
        {
            int? companyId = HttpContext.User.Identity?.GetCompanyId();
            if (!(companyId > 0))
            {
                return BadRequest("شرکت درخواست دهنده معتبر نیست.");
            }
            if(financialPeriodId < 1)
            {
                return BadRequest("شناسه ارتباطی دوره مالی معتبر نیست.");
            }
            InvoiceDto result = await _invoiceService.Find(invoiceNumber, invoiceType, companyId.Value, financialPeriodId, cancellationToken);
            return Ok(result);
        }
        /// <param name="financialPeriodId">شناسه دوره مالی</param>
        [HttpGet("[action]")]
        public async Task<ActionResult<InvoiceDto>> GetIdByNumber(int invoiceNumber, BaseInfoes.InvoiceTypes invoiceType, int financialPeriodId, CancellationToken cancellationToken)
        {
            int? companyId = HttpContext.User.Identity?.GetCompanyId();
            if (!(companyId > 0))
            {
                return BadRequest("شرکت درخواست دهنده معتبر نیست.");
            }
            if (financialPeriodId < 1)
            {
                return BadRequest("شناسه ارتباطی دوره مالی معتبر نیست.");
            }
            long result = await _invoiceService.FindId(invoiceNumber, invoiceType, companyId.Value, financialPeriodId, cancellationToken);
            return Ok(result);
        }
        /// <param name="financialPeriodId">شناسه دوره مالی</param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<ActionResult<InvoiceDto[]>> GetAll(int? financialPeriodId, CancellationToken cancellationToken)
        {
            int? companyId = HttpContext.User.Identity?.GetCompanyId();
            if (!(companyId > 0))
            {
                return BadRequest("شرکت درخواست دهنده معتبر نیست.");
            }
            InvoiceSearchFiltersDto filters = new()
            {
                CompanyId = companyId.Value,
                FinancialPeriodId = financialPeriodId
            };
            var result = await _invoiceService.FindAll(filters, cancellationToken);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<long>> Create(InvoiceDto dto, CancellationToken cancellationToken)
        {
            IIdentity? identity = HttpContext.User.Identity;
            int companyId = identity?.GetCompanyId() ?? 0;
            dto.CreatedById = identity?.GetUserId<int>() ?? 0;
            long id = await _invoiceService.Add(dto, companyId, cancellationToken);
            return Ok(id);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<IEnumerable>> CreateBatch(List<InvoiceDto> dto, CancellationToken cancellationToken)
        {
            var identity = HttpContext.User.Identity;
            int companyId = identity?.GetCompanyId() ?? 0;
            int userId = identity.GetUserId<int>();
            var result = await _invoiceService.Add(dto, companyId, userId, cancellationToken);
            return Ok(result.Select(r => new { r.InvoiceNumber, r.InvoiceType, r.Id }).ToArray());
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<string>> Update(InvoiceDto dto, CancellationToken cancellationToken)
        {
            var identity = HttpContext.User.Identity;
            int companyId = identity?.GetCompanyId() ?? 0;
            int userId = identity.GetUserId<int>();
            dto.UpdatedById = userId;
             await _invoiceService.Edit(dto, companyId, cancellationToken);
            return Ok("Success");
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<string>> UpdateAll(CancellationToken cancellationToken)
        {

            try
            {

                if (!_isUpdatingInvoices.TrySetTrue())
                {
                    return BadRequest("عملیات بروزرسانی هم اکنون در حال انجام است");
                }
                var setting = await _settingRepo.QueryNoTracking.FirstAsync(s => s.SettingName == "LastUpdateDate", cancellationToken);
                if (!string.IsNullOrEmpty(setting?.SettingValue))
                {
                    var lastUpdate = DateTimeOffset.Parse(setting.SettingValue);

                    TimeSpan cooldown = TimeSpan.FromMinutes(30);
                    TimeSpan elapsed = DateTimeOffset.UtcNow - lastUpdate;

                    if (elapsed < cooldown)
                    {
                        TimeSpan remaining = cooldown - elapsed;
                        string message = $"لطفاً {remaining.Minutes} دقیقه و {remaining.Seconds} ثانیه دیگر تلاش کنید.";
                        return BadRequest(message);
                    }
                }
                var identity = HttpContext.User.Identity;
                //int companyId = identity?.GetCompanyId() ?? 0;
                int userId = identity.GetUserId<int>();
                await _invoiceService.UpdateAllInvoices( userId, cancellationToken);

                return Ok("Success");
            }
            catch
            {

                throw;
            }
            finally
            {
                _isUpdatingInvoices.SetFalse();
            }
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult<string>> Delete(long id, CancellationToken cancellationToken)
        {
            var identity = HttpContext.User.Identity;
            int companyId = identity?.GetCompanyId() ?? 0;
            //int userId = identity.GetUserId<int>();
            await _invoiceService.Remove(id, companyId, cancellationToken);
            return Ok("Success");
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> GetLastUpdate(CancellationToken cancellationToken)
        {
            
             var setting = await _settingRepo.QueryNoTracking.FirstAsync(s => s.SettingName == "LastUpdateDate", cancellationToken);
            if (string.IsNullOrEmpty(setting.SettingValue))
            {
                return Ok("تاکنون هیچ بروزرسانی انجام نشده است");
            }
            DateTime utcDateTime = DateTime.Parse(setting.SettingValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            DateTime lastUpdate = utcDateTime.ToLocalTime();
            return Ok(new {Date= lastUpdate.ToPersian() , Time= lastUpdate.ToString("HH:mm") });
        }
    }
}

using AgentsDataView.Common;
using AgentsDataView.Data.Contracts;
using AgentsDataView.Entities;
using AgentsDataView.Entities.DtoModels;
using AgentsDataView.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace AgentsDataView.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController(IReportDataService reportDataRepo, IRepository<Company> companyRepo) : ControllerBase
    {
        private readonly IReportDataService _reportDataRepo = reportDataRepo;
        private readonly IRepository<Company> _companyRepo = companyRepo;

        [HttpGet("[action]")]
        public async Task<ReportResultDto[]> GetProfitReportByCompany(int? provinceId, CancellationToken cancellationToken)
        {
            var provinceIds = await GetProvinceIds(provinceId, cancellationToken);
            var result = await _reportDataRepo.GetProfitReportByCompany(provinceIds, cancellationToken);
            return result;
        }
        [HttpGet("[action]")]
        public async Task<IEnumerable> GetReportByCompanyAndProduct(int? provinceId, CancellationToken cancellationToken)
        {
            var provinceIds = await GetProvinceIds(provinceId, cancellationToken);
            var result = await _reportDataRepo.GetReportByCompanyAndProduct(provinceIds, cancellationToken);
            return result;
        }
        [HttpGet("[action]")]
        public async Task<ReportResultDto[]> GetProfitReportByProvince( CancellationToken cancellationToken)
        {
            var provinceIds = await GetProvinceIds(null, cancellationToken);
            var result = await _reportDataRepo.GetProfitReportByProvince(provinceIds, cancellationToken);
            return result;
        }
        [HttpGet("[action]")]
        public async Task<ReportResultDto[]> GetReportByProvince(int? provinceId, CancellationToken cancellationToken)
        {

           var provinceIds = await GetProvinceIds(provinceId, cancellationToken);
            var result = await _reportDataRepo.GetReportByProvince(provinceIds, cancellationToken);
            return result;
        }
        [HttpGet("[action]")]
        public async Task<IEnumerable> GetReportByProvince_Cumulative(CancellationToken cancellationToken)
        {
            var provinceIds = await GetProvinceIds(0, cancellationToken);
            var result = await _reportDataRepo.GetReportByProvince_Cumulative(provinceIds, cancellationToken);
            return result;
        }

        private async Task<int[]> GetProvinceIds(int? provinceId, CancellationToken cancellationToken)
        {
            if (provinceId > 0)
            {
                return [provinceId.Value];
            }
            else
            {
                var identity = HttpContext.User.Identity;
                int[] companyIds = identity.GetCompanyIds();
             var provinceIds=  await  _companyRepo.QueryNoTracking
                    .Where(c=> c.ProvinceId.HasValue && companyIds.Contains(c.Id)).Select(c=>c.ProvinceId.Value).Distinct().ToArrayAsync(cancellationToken);
                return provinceIds;
            }
        }

    }
}

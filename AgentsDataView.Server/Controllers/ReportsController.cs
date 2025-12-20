using AgentsDataView.Entities.DtoModels;
using AgentsDataView.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace AgentsDataView.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController(IReportDataService reportDataRepo) : ControllerBase
    {
        private readonly IReportDataService _reportDataRepo = reportDataRepo;

        [HttpGet("[action]")]
        public async Task<ReportResultDto[]> GetProfitReportByCompany(int? provinceId, CancellationToken cancellationToken)
        {
            var result = await _reportDataRepo.GetProfitReportByCompany(provinceId, cancellationToken);
            return result;
        }
        [HttpGet("[action]")]
        public async Task<IEnumerable> GetReportByCompanyAndProduct(int? provinceId, CancellationToken cancellationToken)
        {
            var result = await _reportDataRepo.GetReportByCompanyAndProduct(provinceId, cancellationToken);
            return result;
        }
        [HttpGet("[action]")]
        public async Task<ReportResultDto[]> GetProfitReportByProvince( CancellationToken cancellationToken)
        {
            var result = await _reportDataRepo.GetProfitReportByProvince(cancellationToken);
            return result;
        }
        [HttpGet("[action]")]
        public async Task<ReportResultDto[]> GetReportByProvince(int? provinceId, CancellationToken cancellationToken)
        {
            var result = await _reportDataRepo.GetReportByProvince(provinceId, cancellationToken);
            return result;
        }
        [HttpGet("[action]")]
        public async Task<IEnumerable> GetReportByProvince_Cumulative(CancellationToken cancellationToken)
        {
            var result = await _reportDataRepo.GetReportByProvince_Cumulative(cancellationToken);
            return result;
        }

    }
}

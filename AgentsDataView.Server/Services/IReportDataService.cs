using AgentsDataView.Entities.DtoModels;
using System.Collections;

namespace AgentsDataView.Services
{
    public interface IReportDataService
    {
        Task<ReportResultDto[]> GetProfitReportByCompany(int? provinceId, CancellationToken cancellationToken);
        Task<ReportResultDto[]> GetProfitReportByProvince(CancellationToken cancellationToken);
        Task<IEnumerable> GetReportByCompanyAndProduct(int? provinceId, CancellationToken cancellationToken);
        Task<ReportResultDto[]> GetReportByProvince(int? provinceId, CancellationToken cancellationToken);
        Task<IEnumerable> GetReportByProvince_Cumulative(CancellationToken cancellationToken);
    }
}

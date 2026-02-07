using AgentsDataView.Entities.DtoModels;
using System.Collections;

namespace AgentsDataView.Services
{
    public interface IReportDataService
    {
        Task<ReportResultDto[]> GetProfitReportByCompany(int[]? provinceIds, CancellationToken cancellationToken);
        Task<ReportResultDto[]> GetProfitReportByProvince(int[]? provinceIds,CancellationToken cancellationToken);
        Task<IEnumerable> GetReportByCompanyAndProduct(int[]? provinceIds, CancellationToken cancellationToken);
        Task<ReportResultDto[]> GetReportByProvince(int[]? provinceIds, CancellationToken cancellationToken);
        Task<IEnumerable> GetReportByProvince_Cumulative(int[]? provinceIds,CancellationToken cancellationToken);
    }
}

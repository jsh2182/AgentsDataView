using AgentsDataView.Entities;
using AgentsDataView.Entities.DtoModels;

namespace AgentsDataView.Services
{
    public interface IInvoiceService
    {
        Task<long> Add(InvoiceDto dto, int companyId, CancellationToken cancellationToken);
        Task<List<(long Id, int InvoiceNumber, BaseInfoes.InvoiceTypes InvoiceType)>> Add(List<InvoiceDto> dto, int companyId, int userId, CancellationToken cancellationToken);
        Task<long> Edit(InvoiceDto dto, int companyId, CancellationToken cancellationToken);
        Task<InvoiceDto> Find(int invoiceNumber, BaseInfoes.InvoiceTypes invoiceType, int companyId, int financialPeriodId, CancellationToken cancellationToken);
        Task<InvoiceDto> Find(long invoiceId, int companyId, CancellationToken cancellationToken);
        Task<InvoiceDto[]> FindAll(InvoiceSearchFiltersDto dto, CancellationToken cancellationToken);
        Task<long> FindId(int invoiceNumber, BaseInfoes.InvoiceTypes invoiceType, int companyId, int financialPeriodId, CancellationToken cancellationToken);
        Task<Dictionary<int, (DateTimeOffset MaxCreationDate, DateTimeOffset MaxInvoiceDate)>> GetMaxInvoiceDates(CancellationToken cancellationToken);
        Task Remove(long id, int companyId, CancellationToken cancellationToken);
        Task UpdateAllInvoices(int userId, CancellationToken cancellationToken);
    }
}
namespace AgentsDataView.Entities.DtoModels
{
    public class InvoiceSearchFiltersDto
    {

        public int CompanyId { get; set; }
        public int? FinancialPeriodId { get; set; }
        public BaseInfoes.InvoiceTypes? InvoiceType { get; set; }
        public DateTimeOffset? InvoiceDateFrom { get; set; }
        public DateTimeOffset? InvoiceDateTo { get; set; }
    }
}

namespace AgentsDataView.WebFramework.Api
{
    public class CreateCompanyResult
    {
        public int CompanyId { get; set; }
        public int FinancialPeriodId { get; set; }
        public required string UserName { get; set; }
    }
}

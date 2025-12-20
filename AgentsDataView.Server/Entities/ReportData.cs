namespace AgentsDataView.Entities
{
    public class ReportData : BaseEntity<long>
    {
        public int CompanyId { get; set; }
        public required string CompanyName { get; set; }
        public int ProvinceId { get; set; }
        public required string ProvinceName { get; set; }
        public int CityId { get; set; }
        public required string CityName { get; set; }
        public required string ProductCode { get; set; }
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public decimal InputCount { get; set; }
        public decimal InputUnitPrice { get; set; }
        public decimal InputTotalPrice { get; set; }// => InputCount * InputUnitPrice;
        public decimal OutputCount { get; set; }
        public decimal OutputPrice { get; set; }
        public decimal OutputTotalPrice{ get; set; }// OutputCount * OutputPrice;
        /// <summary>
        /// بهای تمام شده کالای فروش رفته
        /// </summary>
        public decimal CostOfGoodsSold { get; set; }
        
        /// <summary>
        /// سود/زیان
        /// </summary>
        public decimal ProfitLosss { get; set; }
        public bool IsSale { get; set; }

    }
}

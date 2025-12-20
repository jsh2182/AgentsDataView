namespace AgentsDataView.Entities.DtoModels
{
    public class ReportResultDto
    {
        /// <summary>
        /// نام استان
        /// </summary>
        public string ProvinceName { get; set; } = "";
        /// <summary>
        /// شناسه شرکت در پایگاه داده اطلاعات تجمیعی
        /// </summary>
        public int CompanyId { get; set; }
        /// <summary>
        /// نام شرکت
        /// </summary>
        public string CompanyName { get; set; } = "";
        /// <summary>
        /// کد کالا
        /// </summary>
        public string ProductCode { get; set; } = "";
        /// <summary>
        /// نام کالا
        /// </summary>
        public string ProductName { get; set; } = "";
        /// <summary>
        /// تعداد وارده(تجمیع کشور)‏
        /// </summary>
        public decimal CountryInputCount { get; set; }
        /// <summary>
        /// مبلغ وارده(تجمیع کشور)‏
        /// </summary>
        public decimal CountryInputTotal { get; set; }
        /// <summary>
        /// تعداد صادره(تجمیع کشور)‏
        /// </summary>
        public decimal CountryOutputCount { get; set; }
        /// <summary>
        /// مبلغ صادره(تجمیع کشور)‏
        /// </summary>
        public decimal CountryOutputTotal { get; set; }
        /// <summary>
        /// تعداد وارده(تجمیع استان)‏
        /// </summary>
        public decimal ProvinceInputCount { get; set; }
        /// <summary>
        /// مبلغ وارده(تجمیع استان)‏
        /// </summary>
        public decimal ProvinceInputTotal { get; set; }
        /// <summary>
        /// تعداد صادره(تجمیع استان)‏
        /// </summary>
        public decimal ProvinceOutputCount { get; set; }
        /// <summary>
        /// مبلغ صادره(تجمیع استان)‏
        /// </summary>
        public decimal ProvinceOutputTotal { get; set; }
        /// <summary>
        /// تعداد صادره(تجمیع شرکت)‏
        /// </summary>        
        public decimal CompanyOutputCount { get; set; }
        /// <summary>
        /// مبلغ صادره(تجمیع شرکت)‏
        /// </summary> 
        public decimal CompanyOutputTotal { get; set; }
        /// <summary>
        /// بهای تمام شده کالای فروش رفته
        /// </summary>
        public decimal CostOfGoodsSold { get; set; }
        /// <summary>
        /// سود/زیان
        /// </summary>
        public decimal ProfitLoss { get; internal set; }
        public int? ProvinceId { get; internal set; }
        public string CompanyCode { get; internal set; }
    }
}

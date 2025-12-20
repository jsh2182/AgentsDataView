namespace AgentsDataView.Entities.DtoModels
{
    public class ProductAggregationDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal InputCount { get; set; }
        public decimal InputTotalPrice { get; set; }
        public decimal OutputCount { get; set; }
        public decimal OutputTotalPrice { get; set; }
        public string CompanyName { get; internal set; }
    }
}

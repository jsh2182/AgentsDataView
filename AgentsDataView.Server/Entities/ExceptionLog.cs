namespace AgentsDataView.Entities
{
    public class ExceptionLog:BaseEntity<int>
    {
        public required string MachineName { get; set; }
        public DateTime Logged { get; set; }
        public required string Message { get; set; }
        public string? Logger { get; set; }
        public string? Exception { get; set; }
        public string? HttpAction { get; set; }
        public string? CallSite { get; set; }
        public string? RequestedURL { get; set; }
    }
}

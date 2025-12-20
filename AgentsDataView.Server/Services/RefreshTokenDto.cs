namespace AgentsDataView.Services
{
    public class RefreshTokenDto
    {
        public long Id { get; set; } = 0;
        public int UserId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpireDate { get; set; }
        public bool IsRevoked { get; set; } = false;

        /// <summary>
        /// برای چرخش توکن
        /// </summary>
        public string? ReplaceByToken { get; set; }
        public DateTime CreationDate { get; set; }
    }
}

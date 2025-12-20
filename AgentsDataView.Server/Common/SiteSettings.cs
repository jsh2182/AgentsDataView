namespace AgentsDataView.Server.Common
{
    public class SiteSettings
    {
        public required JwtSettings JwtSettings { get; set; }
        public required IdentitySettings IdentitySettings { get; set; }
        public string? AllowedCorsOrigins { get; set; }
    }
    public class JwtSettings
    {
        public required string SecretKey { get; set; }
        public required string EncryptKey { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public int NotBeforeMinutes { get; set; }
        public int ExpirationMinutes { get; set; }
        public int RefreshTokenExpirationDays { get; set; }
    }
    public class IdentitySettings
    {
        public bool PasswordRequireDigit { get; set; }
        public int PasswordRequiredLength { get; set; }
        public bool PasswordRequireNonAlphanumic { get; set; }
        public bool PasswordRequireLowercase { get; set; }
        public bool RequireUniqueEmail { get; set; }


    }
}

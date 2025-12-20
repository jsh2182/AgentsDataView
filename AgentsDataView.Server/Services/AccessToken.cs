using AgentsDataView.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace AgentsDataView.Services
{
    public class AccessToken(JwtSecurityToken securityToken, RefreshToken refreshToken)
    {
        public string access_token { get; set; } = new JwtSecurityTokenHandler().WriteToken(securityToken);
        public string token_type { get; set; } = "Bearer";
        public int expires_in { get; set; } = (int)(securityToken.ValidTo - DateTime.UtcNow).TotalSeconds;
        public string? token_for { get; set; } = securityToken.Payload["U_FullName"].ToString();
        public string? u_mobile { get; set; } = securityToken.Payload["U_Mobile"]?.ToString();
        public int comp_id { get; set; }
        public string refresh_token{ get; set; } = refreshToken.Token;
        public int refresh_token_expires_in { get; set; } = (int)(refreshToken.ExpireDate - DateTime.UtcNow).TotalSeconds;
    }
}

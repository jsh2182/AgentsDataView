using AgentsDataView.Entities;
using AgentsDataView.Server.Common;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AgentsDataView.Services
{
    public class JwtService(IOptionsSnapshot<SiteSettings> settings) : IJwtService
    {
        private readonly SiteSettings _siteSetting = settings.Value;

        public TokenBundle Generate(SystemUser user)
        {
            var secretKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.SecretKey); // longer that 32 character
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

            var encryptionkey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.EncryptKey); //must be 16 character
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            //var certificate = new X509Certificate2("d:\\aaaa2.cer"/*, "P@ssw0rd"*/);
            //var encryptingCredentials = new X509EncryptingCredentials(certificate);

            var claims = _getClaims(user);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _siteSetting.JwtSettings.Issuer,
                Audience = _siteSetting.JwtSettings.Audience,
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow.AddMinutes(_siteSetting.JwtSettings.NotBeforeMinutes),
                Expires = DateTime.UtcNow.AddMinutes(_siteSetting.JwtSettings.ExpirationMinutes),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(claims),
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);
            var refreshToken = GenerateRefreshToken(user.Id);

            return  new TokenBundle() { AccessToken = new AccessToken(securityToken, refreshToken), RefreshToken = refreshToken };
        }

        private IEnumerable<Claim> _getClaims(SystemUser user)
        {

            var list = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new ("U_FullName", user.UserFullName),
                new ("U_Mobile", user.UserMobile??""),
                new ("C_Id", (user.CompanyId??0).ToString())

            };

            return  list;
        }

        private RefreshToken GenerateRefreshToken(int userId)
        {
            return new RefreshToken
            {
                Id=0,
                UserId = userId,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpireDate = DateTime.UtcNow.AddDays(_siteSetting.JwtSettings.RefreshTokenExpirationDays),
                CreationDate = DateTime.UtcNow,
                ReplaceByToken = null,
                IsRevoked = false,
            };
        }
    }
}

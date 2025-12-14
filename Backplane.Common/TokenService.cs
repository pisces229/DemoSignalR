using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backplane.Common
{
    public class TokenService(IConfiguration configuration)
    {
        public string CreateToken(string name, TimeSpan timeSpan)
        {
            var securityTokenDescriptor = CreateSecurityTokenDescriptor(name, timeSpan);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            return jwtSecurityTokenHandler.WriteToken(securityToken);
        }

        private SecurityTokenDescriptor CreateSecurityTokenDescriptor(string name, TimeSpan timeSpan) => new()
        {
            Issuer = "Issuer",
            Audience = "Audience",
            //Subject = new ClaimsIdentity(new List<Claim>
            //{
            //    new Claim(JwtRegisteredClaimNames.Iss, _jwtOption.Issuer),
            //    new Claim(JwtRegisteredClaimNames.Aud, _jwtOption.Audience),
            //    new Claim(JwtRegisteredClaimNames.Sub, account),
            //    new Claim(JwtRegisteredClaimNames.Exp, _jwtOption.Expiration.Ticks.ToString()),
            //    new Claim(JwtRegisteredClaimNames.Jti, refreshTokenId),
            //}),
            Claims = new Dictionary<string, object>
            {
                { ClaimTypes.Name, name },
                { ClaimTypes.NameIdentifier, Guid.NewGuid() },
                { ClaimTypes.Role, name }
            },
            Expires = DateTime.UtcNow.Add(timeSpan),
            SigningCredentials =
            new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SigningKey"]!)),
                SecurityAlgorithms.HmacSha256Signature)
        };
    }
}

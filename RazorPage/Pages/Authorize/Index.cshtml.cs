using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RazorPage.Authorize
{
    public class IndexModel(ILogger<IndexModel> logger) : PageModel
    {
        public string AccessToken { get; set; } = null!;
        public void OnGet()
        {
            logger.LogInformation("IndexModel.OnGet");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "Issuer",
                Audience = "Audience",
                //Subject = new ClaimsIdentity(new List<Claim>
                //        {
                //            new Claim(JwtRegisteredClaimNames.Iss, _jwtOption.Issuer),
                //            new Claim(JwtRegisteredClaimNames.Aud, _jwtOption.Audience),
                //            new Claim(JwtRegisteredClaimNames.Sub, account),
                //            new Claim(JwtRegisteredClaimNames.Exp, _jwtOption.Expiration.Ticks.ToString()),
                //            new Claim(JwtRegisteredClaimNames.Jti, refreshTokenId),
                //        }),
                Claims = new Dictionary<string, object>
                {
                    { ClaimTypes.NameIdentifier, "Name" },
                    { ClaimTypes.Role, "Role" }
                },
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("CC5B1B93EDA34B788DD2743CA39BAF89")),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            AccessToken = tokenHandler.WriteToken(securityToken);
        }
    }
}

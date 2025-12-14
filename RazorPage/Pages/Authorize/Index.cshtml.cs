using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RazorPage.Authorize;

public class IndexModel(ILogger<IndexModel> _logger) : PageModel
{
    public string AccessToken { get; set; } = null!;
    public void OnGet()
    {
        _logger.LogInformation("IndexModel.OnGet");
        var securityTokenDescriptor = CreateSecurityTokenDescriptor("Admin", TimeSpan.FromDays(1));
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
        AccessToken = jwtSecurityTokenHandler.WriteToken(securityToken);

        CreateToken();
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
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("CC5B1B93EDA34B788DD2743CA39BAF89")), 
                SecurityAlgorithms.HmacSha256Signature)
    };

    private void CreateToken()
    {
        {
            var securityTokenDescriptor = CreateSecurityTokenDescriptor("Admin", TimeSpan.FromHours(1));
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            _logger.LogInformation("Admin Token: {Token}", jwtSecurityTokenHandler.WriteToken(securityToken));
        }
        {
            var securityTokenDescriptor = CreateSecurityTokenDescriptor("Role", TimeSpan.FromHours(1));
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            _logger.LogInformation("Role Token: {Token}", jwtSecurityTokenHandler.WriteToken(securityToken));
        }
        {
            var securityTokenDescriptor = CreateSecurityTokenDescriptor("Admin", TimeSpan.FromSeconds(1));
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            _logger.LogInformation("Expired Token: {Token}", jwtSecurityTokenHandler.WriteToken(securityToken));
        }
    }

}

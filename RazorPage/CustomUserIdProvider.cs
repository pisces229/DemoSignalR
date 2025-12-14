using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace RazorPage;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection?.GetHttpContext()?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }
    public string GetUserName(HubConnectionContext connection)
    {
        return connection?.GetHttpContext()?.User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
    }
    public string GetRole(HubConnectionContext connection)
    {
        return connection?.GetHttpContext()?.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    }
}

using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Backplane.Common
{
    public static class HubCallerContextExtensions
    {
        public static string? GetUserId(this HubCallerContext hubCallerContext)
        {
            return hubCallerContext?.GetHttpContext()?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }
        public static string? GetUserName(this HubCallerContext hubCallerContext)
        {
            return hubCallerContext?.GetHttpContext()?.User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        }
        public static string? GetRole(this HubCallerContext hubCallerContext)
        {
            return hubCallerContext?.GetHttpContext()?.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        }
    }
}

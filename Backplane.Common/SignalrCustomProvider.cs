using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Backplane.Common;

public class SignalrCustomProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        // Default use ClaimTypes.NameIdentifier
        //var claim = connection?.GetHttpContext()?.User.FindFirst(ClaimTypes.NameIdentifier);
        var claim = connection?.GetHttpContext()?.User.FindFirst(ClaimTypes.Name);
        if (claim != null)
        {
            return claim.Value;
        }
        return null;
    }
}
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace RazorPage
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Name)?.Value;
            //return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}

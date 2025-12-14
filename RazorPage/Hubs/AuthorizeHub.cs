using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RazorPage.Hubs;

// [Authorize]
public class AuthorizeHub(
    ILogger<AuthorizeHub> _logger) 
    : Hub<IAuthorizeClientHub>
{
    public async Task Send1(string message)
    {
        await Clients.All.Receive(message);
    }
    [Authorize]
    //[Authorize(Roles = "Role")]
    //[Authorize(Roles = "Admin")]
    public async Task Send2(string message)
    {
        // CustomUserIdProvider : IUserIdProvider
        await Clients.All.Receive($"{Context.UserIdentifier}: {message}");
        await Clients.All.Receive($"{Context.User?.Identity?.Name}: {message}");
    }
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("AuthorizeHub.OnConnectedAsync:{v}", Context.GetHashCode());
        await base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("AuthorizeHub.OnDisconnectedAsync:{v}", Context.GetHashCode());
        await base.OnDisconnectedAsync(exception);
    }
}
public interface IAuthorizeClientHub
{
    Task Receive(string message);
}

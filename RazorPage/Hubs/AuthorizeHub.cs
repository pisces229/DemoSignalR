using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RazorPage.Repositories;

namespace RazorPage.Hubs
{
    // [Authorize]
    public class AuthorizeHub(ILogger<AuthorizeHub> logger) : Hub
    {
        private readonly ILogger<AuthorizeHub> _logger = logger;
        public async Task Send1(string message)
        {
            await Clients.All.SendAsync("Receive", message);
        }
        [Authorize]
        public async Task Send2(string message)
        {
            await Clients.All.SendAsync("Receive", $"{Context.User?.Identity?.Name}: {message}");
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
}

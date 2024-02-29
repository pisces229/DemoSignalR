using Microsoft.AspNetCore.SignalR;

namespace RazorPage.Hubs
{
    public class GroupHub(ILogger<GroupHub> logger) : Hub
    {
        private readonly ILogger<GroupHub> _logger = logger;
        public async Task JoinGroup(string group, string user)
        {
            await Clients.Group(group).SendAsync("GroupMessage", $"{user} Join Group {group}");
            await Clients.Caller.SendAsync("GroupMessage", $"You Join Group {group}");
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }
        public async Task LeftGroup(string group, string user)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
            await Clients.Group(group).SendAsync("GroupMessage", $"{user} Left Group {group}");
            await Clients.Caller.SendAsync("GroupMessage", $"You Left Group {group}");
        }
        public async Task Send(string message)
        {
            await Clients.All.SendAsync("Receive", message);
        }
        public async Task SendGroup(string group, string user, string message)
        {
            await Clients.Group(group).SendAsync("ReceiveGroup", $"{user}: {message}");
        }
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("GroupHub.OnConnectedAsync:{v}", Context.GetHashCode());
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("GroupHub.OnDisconnectedAsync:{v}", Context.GetHashCode());
            await base.OnDisconnectedAsync(exception);
        }
    }
}

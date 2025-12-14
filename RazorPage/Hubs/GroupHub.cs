using Microsoft.AspNetCore.SignalR;

namespace RazorPage.Hubs;

public class GroupHub(
    ILogger<GroupHub> _logger) 
    : Hub<IGroupClientHub>
{
    public async Task JoinGroup(string group, string user)
    {
        await Clients.Group(group).GroupMessage($"{user} Join Group {group}");
        await Clients.Caller.GroupMessage($"You Join Group {group}");
        await Groups.AddToGroupAsync(Context.ConnectionId, group);
    }
    public async Task LeftGroup(string group, string user)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
        await Clients.Group(group).GroupMessage($"{user} Left Group {group}");
        await Clients.Caller.GroupMessage($"You Left Group {group}");
    }
    public async Task Send(string message)
    {
        await Clients.All.Receive(message);
    }
    public async Task SendGroup(string group, string user, string message)
    {
        await Clients.Group(group).ReceiveGroup($"{user}: {message}");
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
public interface IGroupClientHub
{
    Task GroupMessage(string message);
    Task Receive(string message);
    Task ReceiveGroup(string message);
}

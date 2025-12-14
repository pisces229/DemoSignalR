using Microsoft.AspNetCore.SignalR;

namespace RazorPage.Hubs;

public class BroadcastHub(
    ILogger<BroadcastHub> _logger,
    IHubContext<HomeHub, IHomeClientHub> _hubContext) 
    : Hub<IBroadcastClientHub>
{
    public async Task Send(string message)
    {
        await _hubContext.Clients.All.Receive(message);
    }
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("BroadcastHub.OnConnectedAsync:{v}", Context.GetHashCode());
        await base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("BroadcastHub.OnDisconnectedAsync:{v}", Context.GetHashCode());
        await base.OnDisconnectedAsync(exception);
    }
}
public interface IBroadcastClientHub
{
}

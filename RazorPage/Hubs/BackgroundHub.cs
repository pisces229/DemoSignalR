using Microsoft.AspNetCore.SignalR;

namespace RazorPage.Hubs;

public class BackgroundHub(
    ILogger<BackgroundHub> _logger) 
    : Hub<IBackgroundClientHub>
{
    //public async Task Send(string message)
    //{
    //    await Clients.All.Receive(message);
    //}
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("BackgroundHub.OnConnectedAsync:{v}", Context.GetHashCode());
        await base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("BackgroundHub.OnDisconnectedAsync:{v}", Context.GetHashCode());
        await base.OnDisconnectedAsync(exception);
    }
}
public interface IBackgroundClientHub
{
    Task Receive(string message);
}

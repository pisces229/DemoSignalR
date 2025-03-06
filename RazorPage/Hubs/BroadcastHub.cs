using Microsoft.AspNetCore.SignalR;

namespace RazorPage.Hubs
{
    public class BroadcastHub(ILogger<BroadcastHub> logger,
        IHubContext<HomeHub, IHomeClientHub> homeHubContext) : Hub<IBroadcastClientHub>
    {
        public async Task Send(string message)
        {
            await homeHubContext.Clients.All.Receive(message);
        }
        public override async Task OnConnectedAsync()
        {
            logger.LogInformation("BroadcastHub.OnConnectedAsync:{v}", Context.GetHashCode());
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            logger.LogInformation("BroadcastHub.OnDisconnectedAsync:{v}", Context.GetHashCode());
            await base.OnDisconnectedAsync(exception);
        }
    }
    public interface IBroadcastClientHub
    {
    }
}

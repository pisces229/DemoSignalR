using Microsoft.AspNetCore.SignalR;

namespace RazorPage.Hubs
{
    public class BroadcastHub(ILogger<BroadcastHub> logger,
        IHubContext<IndexHub> hubContext) : Hub
    {
        public async Task Send(string message)
        {
            await hubContext.Clients.All.SendAsync("Receive", message);
            //await Clients.All.SendAsync("Receive", message);
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
}

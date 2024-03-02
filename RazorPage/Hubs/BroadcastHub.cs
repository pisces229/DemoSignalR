using Microsoft.AspNetCore.SignalR;

namespace RazorPage.Hubs
{
    public class BroadcastHub(ILogger<BroadcastHub> logger,
        IHubContext<IndexHub> hubContext) : Hub
    {
        private readonly ILogger<BroadcastHub> _logger = logger;
        private readonly IHubContext<IndexHub> _hubContext = hubContext;
        public async Task Send(string message)
        {
            await _hubContext.Clients.All.SendAsync("Receive", message);
            //await Clients.All.SendAsync("Receive", message);
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
}

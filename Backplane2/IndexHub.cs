using Microsoft.AspNetCore.SignalR;

namespace Backplane
{
    public class IndexHub(ILogger<IndexHub> logger) : Hub<IndexClientHub>
    {
        private readonly ILogger<IndexHub> _logger = logger;
        public async Task Send(string message)
        {
            await Clients.All.Receive($"Backplane2:{message}");
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
    public interface IndexClientHub
    {
        Task Receive(string message);
    }
}

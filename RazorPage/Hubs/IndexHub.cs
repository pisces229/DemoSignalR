using Microsoft.AspNetCore.SignalR;
using RazorPage.Repositories;

namespace RazorPage.Hubs
{
    public class IndexHub(ILogger<IndexHub> logger,
        MessageRepository messageRepository) : Hub
    {
        private readonly ILogger<IndexHub> _logger = logger;
        private readonly MessageRepository _messageRepository = messageRepository;
        public async Task Join()
        {
            await Clients.Caller.SendAsync("History", _messageRepository.Get());
        }
        public async Task Send(string message)
        {
            _messageRepository.Add(message);
            await Clients.All.SendAsync("Receive", message);
        }
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("IndexHub.OnConnectedAsync:{v}", Context.GetHashCode());
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("IndexHub.OnDisconnectedAsync:{v}", Context.GetHashCode());
            await base.OnDisconnectedAsync(exception);
        }
    }
}

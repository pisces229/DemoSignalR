using Microsoft.AspNetCore.SignalR;
using RazorPage.Repositories;

namespace RazorPage.Hubs
{
    public class IndexHub(ILogger<IndexHub> logger,
        MessageRepository messageRepository) : Hub
    {
        public async Task Join()
        {
            var historyMessages = messageRepository.Get();
            historyMessages.Reverse();
            historyMessages = [.. historyMessages.Take(3)];
            historyMessages.Reverse();
            await Clients.Caller.SendAsync("History", historyMessages);
        }
        public async Task Send(string message)
        {
            messageRepository.Add(message);
            await Clients.All.SendAsync("Receive", message);
        }
        public override async Task OnConnectedAsync()
        {
            logger.LogInformation("IndexHub.OnConnectedAsync:{v}", Context.GetHashCode());
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            logger.LogInformation("IndexHub.OnDisconnectedAsync:{v}", Context.GetHashCode());
            await base.OnDisconnectedAsync(exception);
        }
    }
}

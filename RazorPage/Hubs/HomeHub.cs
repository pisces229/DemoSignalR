using Microsoft.AspNetCore.SignalR;
using RazorPage.Repositories;

namespace RazorPage.Hubs;

public class HomeHub(
    ILogger<HomeHub> _logger,
    MessageRepository _messageRepository) 
    : Hub<IHomeClientHub>
{
    public async Task Join()
    {
        var historyMessages = _messageRepository.Get();
        historyMessages.Reverse();
        historyMessages = [.. historyMessages.Take(3)];
        historyMessages.Reverse();
        await Clients.Caller.History(historyMessages);
    }
    public async Task Send(string message)
    {
        _messageRepository.Add(message);
        await Clients.All.Receive(message);
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
public interface IHomeClientHub
{
    Task History(List<string> message);
    Task Receive(string message);
}

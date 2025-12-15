using Microsoft.Extensions.Logging;

namespace Backplane.Common;

public partial class HubService
{
    public async Task Send(string message)
    {
        try
        {
            _logger.LogInformation("[{Name}] HubService Send Message: {Message}", 
                _configuration["Name"]!, message);

            //await _hubContext.Clients.User("User").Receive(message);
            //await _hubContext.Clients.Users(["User1", "User2"]).Receive(message);
            //await _hubContext.Clients.Group("Groups").Receive(message);
            //await _hubContext.Clients.Groups(["Groups1", "Groups2"]).Receive(message);

            // UserId reference SignalrCustomProvider define
            //await _hubContext.Clients.User("Admin")
            //    .Receive($"([{_configuration["Name"]!}] HubService Send Message: {message}.)");
            //await _hubContext.Clients.User(Guid.Empty.ToString())
            //    .Receive($"([{_configuration["Name"]!}] HubService Send Message: {message}.)");

            await _hubContext.Clients.All
                .Receive($"([{_configuration["Name"]!}] HubService Send Message: {message}.)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HubService Send Fail.");
        }
    }
}

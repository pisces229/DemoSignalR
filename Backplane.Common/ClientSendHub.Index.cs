using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Backplane.Common;

public partial class ClientSendHub
{
    public async Task<string> Send(string message)
    {
        try
        {
            _logger.LogInformation("[{Name}] {ConnectionId} ClientSenderHub Send Message: {Message}",
                _configuration["Name"]!, Context.ConnectionId, message);

            //await Clients.User("User").Receive(message);
            //await Clients.Users(["User1", "User2"]).Receive(message);
            //await Clients.Group("Groups").Receive(message);
            //await Clients.Groups(["Groups1", "Groups2"]).Receive(message);

            await Clients.All.Receive($"([{_configuration["Name"]!}] ClientSenderHub Send Message: {message}.)");

            return $"[{_configuration["Name"]!}] Return Success";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ClientSenderHub Send Fail.");
            throw;
        }
    }
    [Authorize]
    public async Task Authorize()
    {
        try
        {
            await Task.Delay(1);

            _logger.LogInformation("ConnectionId:{ConnectionId}", Context.ConnectionId);
            _logger.LogInformation("GetUserId:{GetUserId}", Context.GetUserId());
            _logger.LogInformation("GetUserName:{GetUserName}", Context.GetUserName());
            _logger.LogInformation("GetRole:{GetRole}", Context.GetRole());

            await Task.Delay(1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ClientSenderHub Authorize Fail.");
        }
    }
}

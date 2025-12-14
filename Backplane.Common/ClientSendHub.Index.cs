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

    public async Task<Dto> SendDto(Dto dto)
    {
        try
        {
            await Clients.All.ReceiveDto(dto);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ClientSenderHub SendDto Fail.");
            throw;
        }
    }

}

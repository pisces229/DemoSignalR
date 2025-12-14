using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Backplane.Common;

public partial class ClientSendHub(
    ILogger<ClientSendHub> _logger,
    IConfiguration _configuration)
    : Hub<IClientReceiveHub>
{

    public async Task Ping()
    {
        try
        {
            _logger.LogInformation("Ping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ping Fail.");
        }
    }

    public async Task Heartbeat()
    {
        try
        {
            _logger.LogInformation("Heartbeat");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Heartbeat Fail.");
        }
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            _logger.LogInformation("OnConnectedAsync");

            _logger.LogInformation("ConnectionId:{ConnectionId}", Context.ConnectionId);
            _logger.LogInformation("GetUserId:{GetUserId}", Context.GetUserId());
            _logger.LogInformation("GetUserName:{GetUserName}", Context.GetUserName());
            _logger.LogInformation("GetRole:{GetRole}", Context.GetRole());

            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OnConnectedAsync Fail.");
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            _logger.LogInformation("OnDisconnectedAsync");
            if (exception != null)
            {
                _logger.LogWarning(exception, "");
            }

            _logger.LogInformation("ConnectionId:{ConnectionId}", Context.ConnectionId);
            _logger.LogInformation("GetUserId:{GetUserId}", Context.GetUserId());
            _logger.LogInformation("GetUserName:{GetUserName}", Context.GetUserName());
            _logger.LogInformation("GetRole:{GetRole}", Context.GetRole());

            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OnDisconnectedAsync Fail.");
        }
    }
}

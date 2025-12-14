using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Backplane.Common;

public partial class HubService(
    ILogger<HubService> _logger,
    IHubContext<ClientSendHub, IClientReceiveHub> _hubContext,
    IConfiguration _configuration)
    : IHubService;


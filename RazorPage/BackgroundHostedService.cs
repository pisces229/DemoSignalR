using Microsoft.AspNetCore.SignalR;
using RazorPage.Hubs;

namespace RazorPage;

public class BackgroundHostedService(ILogger<BackgroundHostedService> logger,
    IServiceProvider serviceProvider) : BackgroundService
{
    public override async Task StartAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("BackgroundHostedService is starting.");
        await base.StartAsync(stoppingToken);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("BackgroundHostedService is executing.");
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000);
            var backgroundHub = serviceProvider.GetRequiredService<IHubContext<BackgroundHub>>();
            await backgroundHub.Clients.All.SendAsync("Receive", $"BackgroundHostedService: [{DateTime.Now}]");
        }
    }
    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("BackgroundHostedService is stopping.");
        await base.StopAsync(stoppingToken);
    }
}

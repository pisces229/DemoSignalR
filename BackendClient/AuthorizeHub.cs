using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace BackendClient;

public class AuthorizeHub
{
    public static async Task Run()
    {
        try
        {
            var connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7198/hubs/AuthorizeHub", options =>
            {
                options.SkipNegotiation = true;
                options.Transports = HttpTransportType.WebSockets;
                options.AccessTokenProvider = async () =>
                {
                    return await Task.FromResult("...");
                };
            })
            .WithAutomaticReconnect()
            .ConfigureLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Information);
                logging.AddConsole();
            })
            .Build();

            connection.On<string>("Receive", (message) =>
            {
                Console.WriteLine($"Receive: {message}");
            });

            await connection.StartAsync();

            try
            {
                await connection.InvokeAsync("Send1", "Send1 Backend Client");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }

            try
            {
                await connection.InvokeAsync("Send2", "Send2 Backend Client");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }

            await connection.DisposeAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
        }
    }
}

using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace BackendClient;

public class BackplaneHub
{
    public static async Task Run(string url)
    {
        try
        {
            var connection = new HubConnectionBuilder()
                .WithUrl($"{url}/Hub", options =>
                {
                    //options.SkipNegotiation = true;
                    //options.Transports = HttpTransportType.WebSockets;
                    options.AccessTokenProvider = async () =>
                    {
                        return await Task.FromResult("...");
                    };
                })
                // Handles short network fluctuations (0s, 2s, 10s, 30s)
                .WithAutomaticReconnect()
                //.WithAutomaticReconnect(
                //[
                //    TimeSpan.FromSeconds(0),
                //    TimeSpan.FromSeconds(2),
                //    TimeSpan.FromSeconds(10),
                //    TimeSpan.FromSeconds(30),
                //    TimeSpan.FromMinutes(1),
                //    TimeSpan.FromMinutes(5),
                //])
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                    logging.AddConsole();
                })
                .Build();

            // 1. Listen for reconnecting
            connection.Reconnecting += error =>
            {
                Console.WriteLine($"Connection unstable, reconnecting... ({DateTime.Now})");
                return Task.CompletedTask;
            };

            // 2. Listen for successful reconnection
            connection.Reconnected += connectionId =>
            {
                Console.WriteLine($"Reconnection successful! ({DateTime.Now})");
                return Task.CompletedTask;
            };

            // 3. Listen for connection completely closed (Handles prolonged disconnection)
            connection.Closed += async (error) =>
            {
                Console.WriteLine($"Connection closed. Manually reconnecting in 5 seconds... Error: {error?.Message}");
                await Task.Delay(5000); // Wait for a period to avoid excessive hammering
                try
                {
                    await connection.StartAsync(); // Manually restart
                    Console.WriteLine("Manual reconnection successful!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Manual reconnection failed, the Closed event will be triggered again (potentially). Error: {ex.Message}");
                    // Failure of StartAsync usually does not automatically trigger Closed. Recursive or looping logic might be needed here,
                    // but simply calling StartAsync inside the Closed event is often sufficient to form a basic infinite loop.
                }
            };

            connection.On<string>("Receive", (message) =>
            {
                Console.WriteLine($"{url} BackendClient Receive Message: {message}.");
            });

            // Initial start-up
            try
            {
                await connection.StartAsync();
                Console.WriteLine("Initial connection successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initial connection failed: {ex.Message}. Please check if the server is running.");
                // If the initial connection fails, Closed is usually not triggered. You might need to implement retry logic here.
            }

            var result = await connection.InvokeAsync<string>("Send", $"([{url}] BackendClient Send Message.)");

            Console.WriteLine($"[{url}] Invoke Result: {result}");

            // Keep the main thread from terminating
            await Task.Delay(Timeout.Infinite, CancellationToken.None);

            await connection.DisposeAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
        }
    }
}

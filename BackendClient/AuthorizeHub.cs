using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendClient
{
    public class AuthorizeHub
    {
        public static async Task Run()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7198/hubs/AuthorizeHub", options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        return await Task.FromResult("...");
                    };
                })
                .WithAutomaticReconnect()
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
    }
}

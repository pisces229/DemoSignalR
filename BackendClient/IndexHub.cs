using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendClient
{
    public class IndexHub
    {
        public static async Task Run()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7198/hubs/IndexHub")
                .WithAutomaticReconnect()
                .Build();

            connection.On<string>("Receive", (message) =>
            {
                Console.WriteLine($"Receive: {message}");
            });

            await connection.StartAsync();

            await connection.InvokeAsync("Send", "Send Backend Client");

            await connection.DisposeAsync();
        }
    }
}

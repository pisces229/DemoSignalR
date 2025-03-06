﻿using Microsoft.AspNetCore.SignalR;
using RazorPage.Repositories;

namespace RazorPage.Hubs
{
    public class CustomerHub(ILogger<CustomerHub> logger,
        CustomerRepository customerRepository) : Hub<ICustomerClientHub>
    {
        public Task CustomerJoin()
        {
            customerRepository.CustomerConnectionIds.Add(Context.ConnectionId);
            return Task.CompletedTask;
        }
        public Task CustomerServiceJoin()
        {
            customerRepository.CustomerServiceConnectionId = Context.ConnectionId;
            return Task.CompletedTask;
        }
        public async Task<string> SendToCustomerService(string name, string message)
        {
            await Clients.Caller.CustomerReceive($"{name}: {message}");
            await Clients.Client(customerRepository.CustomerServiceConnectionId).CustomerServiceReceive($"{name}: {message}");
            //await Clients.User(_customerServiceRepository.ConnectionId!).Receive($"{ name }: {message}");
            return "Test";
        }
        public async Task GetCustomers()
        {
            await Clients.Caller.GetCustomersReceive(customerRepository.CustomerConnectionIds);
            //await Clients.User(_customerServiceRepository.ConnectionId!).Receive($"{ name }: {message}");
        }
        public async Task SendToCustomer(string id, string message)
        {
            await Clients.Caller.CustomerServiceReceive($"Customer Service: {message}");
            await Clients.Client(id).CustomerReceive($"Customer Service: {message}");
            //await Clients.User(_customerServiceRepository.ConnectionId!).Receive($"{ name }: {message}");
        }
        public async Task SendToAllCustomer(string message)
        {
            //await Clients.Caller.CustomerServiceReceive($"Customer Service: {message}");
            //await Clients.All.CustomerReceive($"Customer Service: {message}");
            await Clients.Clients(customerRepository.CustomerConnectionIds).CustomerReceive($"Customer Service: {message}");
            await Clients.Clients(customerRepository.CustomerServiceConnectionId).CustomerServiceReceive($"Customer Service: {message}");
        }
        public override async Task OnConnectedAsync()
        {
            logger.LogInformation("GroupHub.OnConnectedAsync:{v}", Context.GetHashCode());
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            customerRepository.CustomerConnectionIds.Remove(Context.ConnectionId);
            logger.LogInformation("GroupHub.OnDisconnectedAsync:{v}", Context.GetHashCode());
            await base.OnDisconnectedAsync(exception);
        }
    }
    public interface ICustomerClientHub
    {
        Task CustomerReceive(string message);
        Task CustomerServiceReceive(string message);
        Task GetCustomersReceive(List<string> message);
        Task Receive(string message);
    }
}

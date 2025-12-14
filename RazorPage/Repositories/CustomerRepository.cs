namespace RazorPage.Repositories;

public class CustomerRepository(ILogger<CustomerRepository> logger)
{
    private readonly ILogger<CustomerRepository> _logger = logger;
    public string CustomerServiceConnectionId { get; set; } = "";
    public List<string> CustomerConnectionIds { get; set; } = [];
}

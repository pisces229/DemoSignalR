namespace RazorPage.Repositories;

public class MessageRepository(ILogger<MessageRepository> logger)
{
    private readonly ILogger<MessageRepository> _logger = logger;
    private readonly List<string> _messages = [];

    public List<string> Get() => _messages;
    public void Add(string message) => _messages.Add(message);
}

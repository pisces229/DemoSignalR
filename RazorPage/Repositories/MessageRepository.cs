namespace RazorPage.Repositories
{
    public class MessageRepository
    {
        private readonly ILogger<MessageRepository> _logger;
        private readonly List<string> _messages = [];
        public MessageRepository(ILogger<MessageRepository> logger)
        {
            _logger = logger;
        }
        public List<string> Get() => _messages;
        public void Add(string message) => _messages.Add(message);
    }
}

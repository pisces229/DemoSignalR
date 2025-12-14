using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPage.Pages.Broadcast;

public class IndexModel(ILogger<IndexModel> _logger) : PageModel
{
    public void OnGet()
    {
        _logger.LogInformation("OnGet");
    }
}

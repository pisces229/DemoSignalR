using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPage.Pages;

public class IndexModel(ILogger<IndexModel> logger) : PageModel
{
    public void OnGet()
    {
        logger.LogInformation("IndexModel.OnGet");
    }
}

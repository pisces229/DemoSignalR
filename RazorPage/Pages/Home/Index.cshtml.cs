using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPage.Pages;

public class HomeModel(ILogger<HomeModel> _logger) : PageModel
{
    public void OnGet()
    {
        _logger.LogInformation("HomeModel.OnGet");
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPage.Pages
{
    public class HomeModel(ILogger<HomeModel> logger) : PageModel
    {
        public void OnGet()
        {
            logger.LogInformation("HomeModel.OnGet");
        }
    }
}

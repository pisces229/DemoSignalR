using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPage.Pages.Group;

public class User1Model : PageModel
{
    public readonly string Name = "User 1";
    public void OnGet()
    {
    }
}

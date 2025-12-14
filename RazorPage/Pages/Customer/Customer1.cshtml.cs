using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPage.Pages.Customer;

public class Customer1Model : PageModel
{
    public readonly string Name = "Customer 1";
    public void OnGet()
    {
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CvWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            var isAuth = User.Identity.IsAuthenticated;
            var name = User.Identity.Name;

            // Logi do Azure Log Stream
            Console.WriteLine($"IsAuthenticated: {isAuth}");
            Console.WriteLine($"Name: {name}");
            Console.WriteLine($"Claims count: {User.Claims.Count()}");

            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
            }
        }

    }
}

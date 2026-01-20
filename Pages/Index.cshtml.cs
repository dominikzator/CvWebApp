using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CvWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<IndexModel> _logger;

        public bool IsDev => _env.IsDevelopment();
        public string EnvName => _env.EnvironmentName;

        public IndexModel(IWebHostEnvironment env, ILogger<IndexModel> logger)
        {
            _env = env;
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

            if (_env.IsDevelopment())
            {
                Console.WriteLine("Environment: Development");
            }
            else if (_env.IsProduction())
            {
                Console.WriteLine("Environment: Production");
            }
        }

    }
}

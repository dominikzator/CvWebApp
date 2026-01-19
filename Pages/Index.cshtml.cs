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
            if (User.Identity.IsAuthenticated)
            {
                _logger.LogInformation("U¿ytkownik zalogowany: {UserName}", User.Identity.Name);

                // Wszystkie claims
                foreach (var claim in User.Claims)
                {
                    _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
                }
            }
            else
            {
                _logger.LogInformation("Brak zalogowanego u¿ytkownika");
            }
        }
    }
}

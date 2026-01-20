using CvWebApp.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Security.Claims;

namespace CvWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<IndexModel> _logger;
        private readonly MainDBContext _context;

        public bool IsDev => _env.IsDevelopment();
        public string EnvName => _env.EnvironmentName;

        public IndexModel(MainDBContext context, IWebHostEnvironment env, ILogger<IndexModel> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        public async Task OnGet()
        {
            string userMail = User.FindFirst(ClaimTypes.Name)?.Value;

            if (_context.Accounts.FirstOrDefault(p => p.Email == userMail) == null)
            {
                _context.Accounts.Add(new Models.Account()
                {
                    Email = userMail
                });
                await _context.SaveChangesAsync();
            }
        }

    }
}

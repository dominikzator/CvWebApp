using CvWebApp.Context;
using CvWebApp.Models;
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

        [BindProperty]  // ← MUSI BYĆ!
        public Note NewNote { get; set; } = new();

        [TempData]
        public string Message { get; set; }

        [TempData]
        public MessageType MessageType { get; set; }

        public string UserMail { get; set; }

        public List<Note> UserNotes { get; set; } = new();

        public IndexModel(MainDBContext context, IWebHostEnvironment env, ILogger<IndexModel> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        public async Task OnGet()
        {
            UserMail = User.FindFirst(ClaimTypes.Name)?.Value;
            UserNotes = _context.Notes.Where(p => p.Owner == UserMail).ToList();

            if (_context.Accounts.FirstOrDefault(p => p.Email == UserMail) == null)
            {
                _context.Accounts.Add(new Models.Account()
                {
                    Email = UserMail
                });
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IActionResult> OnPostCreateAsync()  // ← OnPostCreateAsync!
        {
            Console.WriteLine("OnPostCreateAsync");

            try
            {
                NewNote.CreatedTime = DateTime.UtcNow;
                NewNote.UpdatedTime = DateTime.UtcNow;
                NewNote.Owner = User.FindFirst(ClaimTypes.Name)?.Value!;

                await Console.Out.WriteLineAsync("Trying to Add...");

                await Console.Out.WriteLineAsync("NewNote.Id: " + NewNote.Id);
                await Console.Out.WriteLineAsync("NewNote.Title: " + NewNote.Title);
                await Console.Out.WriteLineAsync("NewNote.Description: " + NewNote.Description);
                await Console.Out.WriteLineAsync("NewNote.Owner: " + NewNote.Owner);
                await Console.Out.WriteLineAsync("NewNote.CreatedTime: " + NewNote.CreatedTime);
                await Console.Out.WriteLineAsync("NewNote.UpdatedTime: " + NewNote.UpdatedTime);

                _context.Notes.Add(NewNote);
                await _context.SaveChangesAsync();

                await Console.Out.WriteLineAsync("Added");

                Message = $"Dodano: {NewNote.Title}";
                MessageType = MessageType.AddSucceeded;
                ModelState.Clear();
                NewNote = new Note();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("ex.Message: " + ex.Message);
                await Console.Out.WriteLineAsync("ex.InnerException.Message: " + ex.InnerException.Message);
            }

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note != null)
            {
                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();

                Message = $"Usunięto notatkę: {note.Title}";
                MessageType = MessageType.DeleteSucceeded;
            }

            return RedirectToPage();
        }
    }
}

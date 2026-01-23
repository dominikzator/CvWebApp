using Azure;
using Azure.Communication.Email;
using CvWebApp.Context;
using CvWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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

        [BindProperty] public Note NewNote { get; set; } = new();
        [BindProperty] public List<NoteView> NoteViews { get; set; } = new();
        [BindProperty] public List<Note> UserNotes { get; set; } = new();

        [TempData] public string Message { get; set; }
        [TempData] public MessageType MessageType { get; set; }

        public string UserMail { get; set; }

        public IndexModel(MainDBContext context, IWebHostEnvironment env, ILogger<IndexModel> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        public async Task OnGet()
        {
            await InitializeData();

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
            try
            {
                NewNote.CreatedTime = DateTime.UtcNow;
                NewNote.UpdatedTime = DateTime.UtcNow;
                NewNote.Owner = User.FindFirst(ClaimTypes.Name)?.Value!;

                _context.Notes.Add(NewNote);
                await _context.SaveChangesAsync();

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
        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            await InitializeData();

            var noteView = NoteViews.FirstOrDefault(p => p.Id == id);

            noteView!.IsEditing = true;
            NewNote = await _context.Notes.FirstOrDefaultAsync(p => p.Id == noteView.Id);

            return Page();
        }
        public async Task<IActionResult> OnPostEditSaveAsync(int id)
        {
            await InitializeData();

            var entity = UserNotes.FirstOrDefault(n => n.Id == id);
            if (entity == null)
            {
                return RedirectToPage();
            }

            entity.Title = NewNote.Title;
            entity.Description = NewNote.Description;
            entity.UpdatedTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            Message = "Notatka została pomyślnie zaktualizowana.";
            MessageType = MessageType.EditSucceeded;

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

        public async Task<IActionResult> OnPostSendMailAsync(int id)
        {
            await Console.Out.WriteLineAsync("Sending Mail...");

            await InitializeData();

            string connectionString = Environment.GetEnvironmentVariable("CommunicationServicesConnectionString");
            if (string.IsNullOrEmpty(connectionString))
            {
                return new BadRequestObjectResult("Brak CommunicationServicesConnectionString");
            }

            var note = await _context.Notes.FindAsync(id);


            var data = new EmailData
            {
                To = UserMail,
                Subject = note.Title + " Note Reminder",
                Message = $"This is a Message about your note! \n Title: {note.Title} \n Description: {note.Description}"
            };

            var emailContent = new EmailContent(data.Subject)
            {
                PlainText = data.Message
            };

            var emailRecipients = new EmailRecipients(new List<EmailAddress> { new(data.To) });

            var emailMessage = new EmailMessage(
                senderAddress: data.From,  // np. "donotreply@twojadomena.azurecomm.net"
                recipients: emailRecipients,
                content: emailContent);

            try
            {
                var emailClient = new EmailClient(connectionString);
                var operation = await emailClient.SendAsync(WaitUntil.Started, emailMessage);
                // Polling na status
                do
                {
                    await Task.Delay(500);
                    await operation.UpdateStatusAsync();
                } while (!operation.HasCompleted);

                string status = operation.HasValue ? operation.Value.Status.ToString() : "Unknown";
                return new OkObjectResult($"Mail wysłany! Operation ID: {operation.Id}, Status: {status}");
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("ex.Message: " + ex.Message);

                return new StatusCodeResult(500);
            }

            return RedirectToPage();
        }

        private async Task InitializeData()
        {
            UserMail = User.FindFirst(ClaimTypes.Name)?.Value!;
            var userNotes = await _context.Notes.Where(p => p.Owner == UserMail).ToListAsync();
            UserNotes = userNotes;
            NoteViews = UserNotes.Select(p => new NoteView
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                UpdateTime = p.UpdatedTime,
                IsEditing = false
            }).ToList();
        }
    }
}

public class EmailData
{
    public string To { get; set; }
    public string From { get; set; } = "donotreply@8044aea5-4316-4b2f-849c-7afa34f35f40.azurecomm.net";
    public string Subject { get; set; }
    public string Message { get; set; }
}
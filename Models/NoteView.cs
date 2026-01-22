namespace CvWebApp.Models
{
    public class NoteView
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime UpdateTime { get; set; }
        public bool IsEditing { get; set; }
    }
}
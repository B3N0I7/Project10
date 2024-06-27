using System.ComponentModel;

namespace MLS_UI.Models
{
    public class NoteDto
    {
        public string Id { get; set; } = string.Empty;
        public string Patient { get; set; } = string.Empty;
        [DisplayName("Note")]
        public string PatientNote { get; set; } = string.Empty;
        public int? PatientId { get; set; }
    }
}

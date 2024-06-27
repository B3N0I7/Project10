namespace PatientDetection.Models
{
    public class PatientWithNotes
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string? PostalAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public List<Note> Notes { get; set; }
        public Dictionary<string, int> TriggersCount { get; set; }
        public int TotalUniqueTrigger { get; set; }
        public int Age { get; set; }
        public string RiskLevel { get; set; }
    }
}

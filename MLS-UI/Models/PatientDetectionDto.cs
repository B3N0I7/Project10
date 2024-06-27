namespace MLS_UI.Models
{
    public class PatientDetectionDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string? PostalAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public List<NoteDto> Notes { get; set; }
        public Dictionary<string, int> TriggersCount { get; set; }
        public int TotalUniqueTrigger { get; set; }
        public int Age { get; set; }
        public string RiskLevel { get; set; }
        //public int Id { get; set; }
        //[Required]
        //[DisplayName("Prénom")]
        //public string FirstName { get; set; }
        //[Required]
        //[DisplayName("Nom")]
        //public string LastName { get; set; }
        //[Required]
        //[DisplayName("Date de naissance")]
        //[DataType(DataType.Date)]
        //public DateTime BirthDate { get; set; }
        //[Required]
        //[DisplayName("Genre")]
        //public string Gender { get; set; }
        //[DisplayName("Adresse postale")]
        //public string? PostalAddress { get; set; }
        //[DisplayName("Numéro de téléphone")]
        //public string? PhoneNumber { get; set; }
        //public string? Note { get; set; }
    }
}

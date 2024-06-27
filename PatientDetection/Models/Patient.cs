using System.ComponentModel.DataAnnotations;

namespace PatientDetection.Models
{
    public class Patient
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [Required]
        public string Gender { get; set; }
        public string? PostalAddress { get; set; }
        public string? PhoneNumber { get; set; }
    }
}

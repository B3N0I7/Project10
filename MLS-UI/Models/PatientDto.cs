using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MLS_UI.Models
{
    public class PatientDto
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Prénom")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Nom")]
        public string LastName { get; set; }
        [Required]
        [DisplayName("Date de naissance")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        [Required]
        [DisplayName("Genre")]
        public string Gender { get; set; }
        [DisplayName("Adresse postale")]
        public string? PostalAddress { get; set; }
        [DisplayName("Numéro de téléphone")]
        public string? PhoneNumber { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace RVS_Aplikacija.ViewModels
{
    public class PrijavaViewModel
    {
        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage="Unesite validan email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Lozinka je obavezna")]
        [DataType(DataType.Password)]
        public string Sifra { get; set; }

    }
}

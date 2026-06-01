using System.ComponentModel.DataAnnotations;

namespace RVS_Aplikacija.ViewModels
{
    public class RegistracijaViewModel
    {
        [Required(ErrorMessage = "Email adresa je obavezna.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [MinLength(6, ErrorMessage = "Dužina lozinke mora biti minimum 6 karaktera.")]
        public string Lozinka { get; set; }

        [Required(ErrorMessage = "Potvrda lozinke je obavezna.")]
        [MinLength(6, ErrorMessage = "Dužina lozinke mora biti minimum 6 karaktera.")]
        public string PotvrdaLozinke { get; set; }

        [Required(ErrorMessage = "Izaberite tip korisnika.")]
        public string Tip { get; set; } = "F";

        // Fizičko lice
        public string? Ime { get; set; }
        public string? Prezime { get; set; }
        public string? Jmbg { get; set; }

        // Pravno lice
        public string? Naziv { get; set; }
        public string? MaticniBroj { get; set; }

        [Required(ErrorMessage = "Mesto je obavezno.")]
        public Guid MestoID { get; set; }

        [Required(ErrorMessage = "Ulica i kućni broj su obavezni.")]
        public string? UlicaBroj { get; set; }

        [Phone(ErrorMessage = "Neispravan format broja telefona.")]
        public string? Telefon { get; set; }

        public string? Stan { get; set; }
    }
}
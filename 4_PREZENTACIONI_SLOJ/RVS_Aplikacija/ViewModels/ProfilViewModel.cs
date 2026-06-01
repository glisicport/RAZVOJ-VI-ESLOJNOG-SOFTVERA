using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RVS_Aplikacija.ViewModels
{
    public class ProfilViewModel : IValidatableObject
    {
        public Guid Id { get; set; }

        public string? Ime { get; set; }

        public string? Prezime { get; set; }

        public string? Naziv { get; set; }

        public bool PrikazNaziva =>
            string.IsNullOrWhiteSpace(Ime) ||
            string.IsNullOrWhiteSpace(Prezime);

        [Phone(ErrorMessage = "Telefon nije u ispravnom formatu.")]
        public string? Telefon { get; set; }

        [Required(ErrorMessage = "Mesto je obavezno.")]
        public Guid MestoID { get; set; }

        [Required(ErrorMessage = "Ulica i broj su obavezni.")]
        public string? UlicaBroj { get; set; }

        public string? Stan { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!PrikazNaziva)
            {
                if (string.IsNullOrWhiteSpace(Ime))
                {
                    yield return new ValidationResult("Ime je obavezno.", new[] { nameof(Ime) });
                }

                if (string.IsNullOrWhiteSpace(Prezime))
                {
                    yield return new ValidationResult("Prezime je obavezno.", new[] { nameof(Prezime) });
                }
            }
        }
    }
}
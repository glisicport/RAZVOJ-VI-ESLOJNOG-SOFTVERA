using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlasa.KlasePodatakaEF.ModeliEF
{
    [Table("Lice")]
    public class LiceModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Tip { get; set; } = null!; // F / P

        public string? Ime { get; set; }
        public string? Prezime { get; set; }

        public string? Naziv { get; set; }

        public string? Jmbg { get; set; }
        public string? Pib { get; set; }
        public string? MaticniBroj { get; set; }

        public string? Telefon { get; set; }
        public Guid? AdresaId { get; set; }

        public KorisnikModel? Korisnik { get; set; }

        public AdresaModel? adresa { get; set; }
    }
}

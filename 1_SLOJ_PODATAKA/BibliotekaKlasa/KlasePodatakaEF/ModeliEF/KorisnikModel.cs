using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlasa.KlasePodatakaEF.ModeliEF
{
    [Table("korisnik")]

    public class KorisnikModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string LozinkaHash { get; set; } = null!;
        [Required]
        public string Rola { get; set; }
        public DateTime KreiranU { get; set; } = DateTime.UtcNow;
        public Guid LiceID { get; set; }

        // NAVIGATION
        public LiceModel? Lice { get; set; }
    }
}

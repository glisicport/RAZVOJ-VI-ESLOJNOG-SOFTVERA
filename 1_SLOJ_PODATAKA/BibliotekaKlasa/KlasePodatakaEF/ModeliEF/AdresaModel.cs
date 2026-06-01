using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotekaKlasa.KlasePodatakaEF.ModeliEF
{
    [Table("adresa")]
    public class AdresaModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("mesto_id")]
        public Guid MestoId { get; set; }

        public int? Sprat { get; set; }

        public string? Stan { get; set; }

        [Column("ulicaIBroj")]
        public string UlicaIBroj { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlasa.KlasePodataka.ApiPozivi
{
    public class PoreskiPodaciDto
    {
        public DateTime? DatumNastanka { get; set; }
        public DateTime? DatumPrestanka { get; set; }

        public bool ImaKredit { get; set; }
        public bool ImaOslobodjenje { get; set; }

        public string? Dokazi { get; set; }
        public string? Napomena { get; set; }

        public decimal? Osnova { get; set; }
        public string? kredit_osnov { get; set; }
        public string? oslobodjenje_osnov { get; set; }
        public decimal? Udeo { get; set; }
        public bool? Oslobodjenje { get; set; }
    }
}

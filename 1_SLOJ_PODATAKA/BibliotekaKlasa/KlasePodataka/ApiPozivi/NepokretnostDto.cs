using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlasa.KlasePodataka.ApiPozivi
{
    public class NepokretnostDto
    {
        public short? VrstaId { get; set; }
        public Guid KatastarskaOpstinaId { get; set; }
        public string ulicaIbroj { get; set; }
        public string? BrKatParcele { get; set; }
        public int? GodinaIzgradnje { get; set; }
        public decimal KorisnaPovrsina { get; set; }
        public decimal UkupnaPovrsina { get; set; }
        public decimal UdeoObveznika { get; set; }
        public bool ImaPoreskiKredit { get; set; }
        public bool ImaPoreskoOslobodjenje { get; set; }
        public string? OslobodjenjeOsnov { get; set; }
        public string? Napomena { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlasa.KlasePodataka.ApiPozivi
{
   
        public class NepokretnostOdziv
        {
            public Guid? NepokretnostId { get; set; }
            public Guid? PrijavaId { get; set; }

            public short? VrstaId { get; set; }
            public short? Rbr { get; set; }
        public string? SifraVrste { get; set; }

        public Guid? KatastarskaOpstinaId { get; set; }
            public string? KatastarskaOpstinaNaziv { get; set; }

            public string? UlicaIbroj { get; set; }
            public string? BrKatParcele { get; set; }

            public short? GodinaIzgradnje { get; set; }

            public decimal? KorisnaPovrsina { get; set; }
            public decimal? UkupnaPovrsina { get; set; }
            public decimal? UdeoObveznika { get; set; }

            public bool? ImaPoreskiKredit { get; set; }
            public bool? ImaPoreskoOslobodjenje { get; set; }

            public string? OslobodjenjeOsnov { get; set; }
            public string? Napomena { get; set; }

        }
    }


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlasa.KlasePodataka.ApiPozivi
{
    public class PoreskaPrijavaOdziv
    {
        public Guid PoreskaPrijavaId { get; set; }

        public Guid AdresaId { get; set; }

        public Guid ObveznikLiceId { get; set; }
        public string? ObveznikLiceNaziv { get; set; }

        public string? ObveznikSprat { get; set; }
        public string? ObveznikStan { get; set; }
        public string? ObveznikUlicaBroj { get; set; }
        public string? ObveznikMesto { get; set; }
        public int? ObveznikPtt { get; set; }

        public Guid? PodnosilacLiceId { get; set; }
        public string? PodnosilacLiceNaziv { get; set; }

        public bool JeIzmenjena { get; set; }

        public short VrstaPravaId { get; set; }
        public string VrstaPravaNaziv { get; set; } = string.Empty;

        public Guid JlsId { get; set; }
        public string? JlsNaziv { get; set; }

        public DateTime? DanNastankaPoreskeObaveze { get; set; }
        public DateTime? DanPrestankaPoreskeObaveze { get; set; }

        public short? RazlogPrestankaId { get; set; }

        public bool PpImaPoreskiKredit { get; set; }
        public string? PpKreditOsnov { get; set; }

        public bool PpImaPoreskoOslobodjenje { get; set; }
        public string? PpOslobodjenjeOsnov { get; set; }

        public DateTime? DatumPodnosenja { get; set; }

        public string? PpNapomena { get; set; }

        public DateTime KreiranaU { get; set; }
        public DateTime? AzuriranaU { get; set; }

        public string? PopisPrilozenihDokaza { get; set; }
        public string? OrganizacionaJedinica { get; set; }
        public string? JMBG { get; set; }
        public string? PIB { get; set; }
        public string? Telefon { get; set; }
        public string? Email { get; set; }


        public List<NepokretnostOdziv> Nepokretnosti { get; set; } = new();
    }
}

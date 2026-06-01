using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlasa.KlasePodataka.Modeli
{
    public class PoreskaPrijava
    {
        public Guid Id { get; set; }

        public string ObveznikTip { get; set; }
        public string PodnosilacNaziv { get; set; }

        public string VrstaPravaId { get; set; }
        public Guid JlsId { get; set; }

        public string NoviObveznikTip { get; set; }
        public string NoviObveznikIme { get; set; }
        public string NoviObveznikPrezime { get; set; }

        public bool ImaPoreskiKredit { get; set; }
        public bool ImaPoreskoOslobodjenje { get; set; }
        public string? kredit_osnov { get; set; }
        public string? oslobodjenje_osnov { get; set; }

        public DateTime DanNastankaPoreskeObaveze { get; set; }
        public DateTime DanPrestankaPoreskeObaveze { get; set; }
        public DateTime DatumPodnosenja { get; set; }

        public string Napomena { get; set; }
        public string OrganizacionaJedinica { get; set; }
        public string PopisPrilozenihDokazas { get; set; }

        public List<Nepokretnost> Objekti { get; set; }
    }
}

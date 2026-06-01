using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlasa.KlasePodataka.ApiPozivi
{
    public class PoreskaPrijavaPoziv
    {
        public ObveznikDto? Obveznik { get; set; }
        public string? Podnosilac { get; set; }
        public string? OrganizacionaJedinica { get; set; }
        public short? VrstaPravaId { get; set; }
        public Guid? JlsId { get; set; }
        public PoreskiPodaciDto? PoreskiPodaci { get; set; }
        public List<NepokretnostDto>? Objekti { get; set; }
    }
}

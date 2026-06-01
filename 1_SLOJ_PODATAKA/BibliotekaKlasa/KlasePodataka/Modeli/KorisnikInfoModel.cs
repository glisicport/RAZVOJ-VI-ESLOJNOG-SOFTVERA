using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlasa.KlasePodataka.Modeli
{
    public class KorisnikInfoModel
    {
        public string Email { get; set; }
        public string ImePrezimeIliNaziv { get; set; }

        public string LozinkaHash { get; set; }
        public System.Guid Id { get; set; }
        public string Rola {  get; set; }
    }
}

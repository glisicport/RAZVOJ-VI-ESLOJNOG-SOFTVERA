using BibliotekaKlasa.KlasePodatakaEF.ModeliEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlasa.KlasePodataka.ApiPozivi
{
    public class RegistracijaPoziv
    {
        public string Email { get; set; }
        public string Lozinka { get; set; }

        public string Tip { get; set; }
        public string? Ime { get; set; }
        public string? Prezime { get; set; }
        public string? Jmbg { get; set; }
        public string? MaticniBroj { get; set; }
    }
}

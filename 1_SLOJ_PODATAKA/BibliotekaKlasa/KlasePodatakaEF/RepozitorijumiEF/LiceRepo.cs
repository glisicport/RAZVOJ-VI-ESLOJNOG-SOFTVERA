using BibliotekaKlasa.KlasePodatakaEF.KontekstEF;
using BibliotekaKlasa.KlasePodatakaEF.ModeliEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaKlasa.KlasePodatakaEF.RepozitorijumiEF
{
    internal class LiceRepo
    {
        private readonly AppDbContext _appKontekst;

        public LiceRepo(AppDbContext kontekst)
        {
            _appKontekst = kontekst;
        }

      
    }
}

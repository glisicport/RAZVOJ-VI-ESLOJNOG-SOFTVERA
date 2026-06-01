using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using BibliotekaKlasa.KlasePodatakaEF.RepozitorijumiEF;
using BibliotekaKlasa.TehnoloskeKlase;
using Microsoft.AspNetCore.Mvc;
using RVS_Aplikacija.ViewModels;

namespace RVS_Aplikacija.Controllers
{
    public class SifarniciController : Controller
    {
        private readonly KorisnikRepo _repo;
        private readonly MestoRepoziturijum _Mestorepo;
        private readonly KonekcijaKlasa _konekcijaObjekat;

        public SifarniciController(KorisnikRepo repo, IConfiguration konfiguracija)
        {
            string konekcioniString = konfiguracija.GetConnectionString("KonekcioniString");

            _repo = repo;
            _konekcijaObjekat = new KonekcijaKlasa(konekcioniString);

            _Mestorepo = new MestoRepoziturijum(_konekcijaObjekat);
        }

        public IActionResult Index()
        {
            _konekcijaObjekat.OtvoriKonekciju();

            var mesta = _Mestorepo.DajSvaMesta();

            _konekcijaObjekat.ZatvoriKonekciju();

            var model = mesta.Select(x => new MestoViewModel
            {
                Id = x.Id,
                Naziv = x.Naziv,
                Ptt = x.Ptt
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Dodaj(MestoViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            _konekcijaObjekat.OtvoriKonekciju();

            var postojiPtt = _Mestorepo.DajSvaMesta()
                .Any(x => x.Ptt == model.Ptt);

            if (postojiPtt)
            {
                ModelState.AddModelError("Ptt", "PTT već postoji u bazi.");
                _konekcijaObjekat.ZatvoriKonekciju();
                return View("Index", GetModel());
            }

            _Mestorepo.DodajMesto(new MestoModel
            {
                Id = Guid.NewGuid(),
                Naziv = model.Naziv,
                Ptt = model.Ptt
            });

            _konekcijaObjekat.ZatvoriKonekciju();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Izmeni(MestoViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            _konekcijaObjekat.OtvoriKonekciju();

            var postojiPtt = _Mestorepo.DajSvaMesta()
                .Any(x => x.Ptt == model.Ptt && x.Id != model.Id);

            if (postojiPtt)
            {
                ModelState.AddModelError("Ptt", "PTT već postoji u bazi.");
                _konekcijaObjekat.ZatvoriKonekciju();
                return View("Index", GetModel());
            }

            _Mestorepo.IzmeniMesto(new MestoModel
            {
                Id = model.Id,
                Naziv = model.Naziv,
                Ptt = model.Ptt
            });

            _konekcijaObjekat.ZatvoriKonekciju();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Obrisi(Guid id)
        {
            _konekcijaObjekat.OtvoriKonekciju();

            _Mestorepo.ObrisiMesto(id);

            _konekcijaObjekat.ZatvoriKonekciju();

            return RedirectToAction("Index");
        }

        private List<MestoViewModel> GetModel()
        {
            _konekcijaObjekat.OtvoriKonekciju();

            var mesta = _Mestorepo.DajSvaMesta();

            _konekcijaObjekat.ZatvoriKonekciju();

            return mesta.Select(x => new MestoViewModel
            {
                Id = x.Id,
                Naziv = x.Naziv,
                Ptt = x.Ptt
            }).ToList();
        }
    }
}
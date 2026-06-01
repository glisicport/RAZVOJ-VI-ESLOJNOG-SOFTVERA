using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using BibliotekaKlasa.KlasePodatakaEF.ModeliEF;
using BibliotekaKlasa.KlasePodatakaEF.RepozitorijumiEF;
using BibliotekaKlasa.TehnoloskeKlase;
using BibliotekaKlasa.TehnoloskeKlase.PomocneFunkcije;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RVS_Aplikacija.ViewModels;
using System.Collections;
using System.Security.Claims;

namespace RVS_Aplikacija.Controllers
{
    public class NalogController : Controller
    {
        private readonly KorisnikRepo _repo;
        private readonly MestoRepoziturijum _Mestorepo;
        private readonly KonekcijaKlasa _konekcijaObjekat;

        public NalogController(KorisnikRepo repo, IConfiguration konfiguracija)
        {
            string konekcioniString = konfiguracija.GetConnectionString("KonekcioniString");

            _repo = repo;
            _konekcijaObjekat = new KonekcijaKlasa(konekcioniString);

            _Mestorepo = new MestoRepoziturijum(_konekcijaObjekat);

        }
        [HttpPost]
        public async Task<IActionResult> Izmeni(ProfilViewModel model)
        {
            var korisnikId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(korisnikId))
                return RedirectToAction("Prijava");
            _konekcijaObjekat.OtvoriKonekciju();
            ViewBag.Mesta = _Mestorepo.DajSvaMesta();
            _konekcijaObjekat.ZatvoriKonekciju();
          

            if (!ModelState.IsValid)
                return View("Index",model);

            var korisnik = new KorisnikModel
            {
                Id = model.Id,

                Lice = new LiceModel
                {
                    Ime = model.Ime,
                    Prezime = model.Prezime,
                    Naziv = model.Naziv,
                    Telefon = model.Telefon,

                    adresa = new AdresaModel
                    {
                        MestoId = model.MestoID,
                        UlicaIBroj = model.UlicaBroj,
                        Stan = model.Stan
                    }
                }
            };

            await _repo.IzmeniKorisnika(korisnik);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Index()
        {
            var korisnikId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(korisnikId))
                return RedirectToAction("Prijava");
            _konekcijaObjekat.OtvoriKonekciju();
            ViewBag.Mesta = _Mestorepo.DajSvaMesta();
            _konekcijaObjekat.ZatvoriKonekciju();
            var korisnik = await _repo.DohvatiPoId(Guid.Parse(korisnikId));

            if (korisnik == null)
                return NotFound();

            var vm = new ProfilViewModel
            {
                Id = korisnik.Id,
                Ime = korisnik.Lice?.Ime,
                Prezime = korisnik.Lice?.Prezime,
                Naziv = korisnik.Lice?.Naziv,
                Telefon = korisnik.Lice?.Telefon,

                MestoID = korisnik.Lice?.adresa?.MestoId ?? Guid.Empty,
                UlicaBroj = korisnik.Lice?.adresa?.UlicaIBroj,
                Stan = korisnik.Lice?.adresa?.Stan
            };
           

            return View(vm);
        }
        public IActionResult Prijava()
        {
            _konekcijaObjekat.OtvoriKonekciju();
            ViewBag.Mesta = _Mestorepo.DajSvaMesta();
            _konekcijaObjekat.ZatvoriKonekciju();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Prijava(PrijavaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var korisnik = await _repo.DohvatiPoEmailu(model.Email);

            if (korisnik == null)
            {
                ModelState.AddModelError(string.Empty, "Neispravan email ili lozinka");
                return View(model);
            }

            bool lozinkaTacna = FunkcijeLozinke.ProveriLozinku(
                model.Sifra,
                korisnik.LozinkaHash
            );

            if (!lozinkaTacna)
            {
                ModelState.AddModelError(string.Empty, "Neispravan email ili lozinka");
                return View(model);
            }

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, korisnik.Email),
                    new Claim(ClaimTypes.NameIdentifier, korisnik.Id.ToString()),
                    new Claim(ClaimTypes.Role, korisnik.Rola),

                    new Claim("Naziv", korisnik.ImePrezimeIliNaziv)
                };

            var identitet = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var princip = new ClaimsPrincipal(identitet);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                princip
            );

            return RedirectToAction("Index","Pocetna");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OdjaviSe()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index");
        }
        public IActionResult Registracija()
        {
            _konekcijaObjekat.OtvoriKonekciju();
            ViewBag.Mesta = _Mestorepo.DajSvaMesta();
            if (ViewBag.Mesta == null)
            {
                ViewBag.Mesta = new List<MestoModel>();
            }
            _konekcijaObjekat.ZatvoriKonekciju();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registracija(RegistracijaViewModel model)
        {
            _konekcijaObjekat.OtvoriKonekciju();
            ViewBag.Mesta = _Mestorepo.DajSvaMesta();
            _konekcijaObjekat.OtvoriKonekciju();

            if (!ModelState.IsValid)
            {
             
                return View(model);
            }

            if (await _repo.PostojiKorisnikSaEmailom(model.Email))
            {
                ModelState.AddModelError("Email", "Email već postoji");
                return View(model);
            }

            if (model.Tip == "F")
            {
                if (await _repo.PostojiJMBGVecUpisan(model.Jmbg))
                {
                    ModelState.AddModelError("Jmbg", "JMBG već postoji");
                    return View(model);
                }
            }

            if (model.Tip == "P")
            {
                if (await _repo.PostojiImeFirmeVecUpisano(model.Naziv))
                {
                    ModelState.AddModelError("Naziv", "Firma već postoji");
                    return View(model);
                }

                if (await _repo.PostojiMaticniBrojVecUpisan(model.MaticniBroj))
                {
                    ModelState.AddModelError("MaticniBroj", "Matični broj već postoji");
                    return View(model);
                }
            }

            var korisnik = new KorisnikModel
            {
                Id = Guid.NewGuid(),
                Email = model.Email,
                LozinkaHash = FunkcijeLozinke.HashujLozinku(model.Lozinka),
                KreiranU = DateTime.UtcNow,
                Rola = "korisnik"
            };

            var lice = new LiceModel
            {
                Id = Guid.NewGuid(),
                Tip = model.Tip,

                Ime = model.Tip == "F" ? model.Ime : null,
                Prezime = model.Tip == "F" ? model.Prezime : null,
                Jmbg = model.Tip == "F" ? model.Jmbg : null,

                Naziv = model.Tip == "P" ? model.Naziv : null,
                Pib = model.Tip == "P" ? model.MaticniBroj : null,
                Telefon = model.Telefon,
                Korisnik = korisnik,
                adresa = new AdresaModel { 
                    MestoId = model.MestoID,
                    UlicaIBroj = model.UlicaBroj,
                    Stan = model.Stan
                }
            };


            await _repo.Dodaj(korisnik, lice);

            return RedirectToAction("Index");
        }
    }
}
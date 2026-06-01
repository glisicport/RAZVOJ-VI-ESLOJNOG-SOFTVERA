using BibliotekaKlasa.KlasePodataka.ApiPozivi;
using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using BibliotekaKlasa.TehnoloskeKlase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RVS_Aplikacija.ViewModels;

namespace RVS_Aplikacija.Controllers
{
    public class PoreskaPrijavaController : Controller
    {
        private readonly VrstaPravaRepozitorijum _repozitorijumPrava;
        private readonly KonekcijaKlasa _konekcija;
        private readonly MestoRepoziturijum _repozitorijumMesta;
        private readonly HttpClient _httpKlijent;

        public PoreskaPrijavaController(
            IConfiguration konfiguracija,
            IHttpClientFactory factory)
        {
            string konekcioniString =
                konfiguracija.GetConnectionString("KonekcioniString");

            _konekcija = new KonekcijaKlasa(konekcioniString);
            _repozitorijumPrava = new VrstaPravaRepozitorijum(konekcioniString);
            _repozitorijumMesta = new MestoRepoziturijum(_konekcija);

            _httpKlijent = factory.CreateClient();
        }
        [HttpPost]
        public async Task<IActionResult> Obrisi(Guid poreska_prijava_id)
        {
            try
            {
                string adresaApiPoziva =
                    $"http://localhost:5089/api/PoreskaPrijava?prijavaId={poreska_prijava_id}";
                Console.WriteLine("USAO:::");
                Console.WriteLine(poreska_prijava_id);
                HttpResponseMessage odgovor =
                    await _httpKlijent.DeleteAsync(adresaApiPoziva);

                if (!odgovor.IsSuccessStatusCode)
                {
                    TempData["Greska"] =
                        "Doslo je do greske prilikom brisanja poreske prijave.";

                    return RedirectToAction(nameof(Pregled));
                }

                TempData["Uspeh"] =
                    "Poreska prijava je uspesno obrisana.";

                return RedirectToAction(nameof(Pregled));
            }
            catch
            {
                TempData["Greska"] =
                    "Doslo je do neocekivane greske.";

                return RedirectToAction(nameof(Pregled));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Pregled(
            string? pretraga,
            string? jls,
            string? obveznik,
            string? vrstaPrava,
            bool? samoIzmenjene,
            char? stampa,
            Guid? id)
        {
            string? korisnikIdString =
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            Guid korisnikId = Guid.Parse(
                korisnikIdString ??
                "00000000-0000-0000-0000-000000000000");

            string adresaApiPoziva =
                $"http://localhost:5089/api/PoreskaPrijava/VratiPrijavuPoKorisnikID?korisnikId={korisnikId}";

            List<PoreskaPrijavaOdziv> poreskePrijave;

            try
            {
                poreskePrijave =
                    await _httpKlijent.GetFromJsonAsync<List<PoreskaPrijavaOdziv>>(adresaApiPoziva)
                    ?? new List<PoreskaPrijavaOdziv>();
            }
            catch
            {
                poreskePrijave = new List<PoreskaPrijavaOdziv>();
            }

            if (!string.IsNullOrWhiteSpace(pretraga))
            {
                poreskePrijave = poreskePrijave
                    .Where(prijava =>
                        prijava.PoreskaPrijavaId.ToString()
                            .Contains(pretraga, StringComparison.OrdinalIgnoreCase)
                        ||
                        (prijava.ObveznikLiceNaziv ?? "")
                            .Contains(pretraga, StringComparison.OrdinalIgnoreCase)
                        ||
                        (prijava.PpNapomena ?? "")
                            .Contains(pretraga, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(jls))
            {
                poreskePrijave = poreskePrijave
                    .Where(prijava =>
                        (prijava.JlsNaziv ?? "")
                            .Contains(jls, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(obveznik))
            {
                poreskePrijave = poreskePrijave
                    .Where(prijava =>
                        (prijava.ObveznikLiceNaziv ?? "")
                            .Contains(obveznik, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(vrstaPrava))
            {
                poreskePrijave = poreskePrijave
                    .Where(prijava =>
                        (prijava.VrstaPravaNaziv ?? "")
                            .Contains(vrstaPrava, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (samoIzmenjene == true)
            {
                poreskePrijave = poreskePrijave
                    .Where(prijava => prijava.JeIzmenjena)
                    .ToList();
            }

            if (stampa == 'l')
            {
                return View("Stampa", poreskePrijave);
            }

            if (stampa == 'j' && id.HasValue)
            {
                var prijava =
                    poreskePrijave.FirstOrDefault(x =>
                        x.PoreskaPrijavaId == id.Value);

                return View(
                    "StampaDokument",
                    new List<PoreskaPrijavaOdziv> { prijava });
            }

            return View(poreskePrijave);
        }

        public IActionResult Index()
        {
            try
            {
                _konekcija.OtvoriKonekciju();

                var vrstePrava =
                    _repozitorijumPrava.DajSvaPrava();

                var jlsLista =
                    _repozitorijumMesta.DajSveJLS();

                var katastarskeOpstine =
                    _repozitorijumMesta.DajSveOpstine();

                var vrsteObjekata =
                    _repozitorijumMesta.DajSveVrsteObjekata();

                ViewBag.Jls =
                    new SelectList(jlsLista, "Id", "Naziv");

                ViewBag.VrsteObjekata =
                    new SelectList(vrsteObjekata, "Id", "Naziv");

                var model = new PoreskaPrijavaViewModel
                {
                    VrstePrava = vrstePrava
                        .Select(pravo => new VrstaPravaModel
                        {
                            Id = pravo.Id,
                            Sifra = pravo.Sifra,
                            Naziv = pravo.Naziv
                        })
                        .ToList(),

                    opstine = katastarskeOpstine
                        .Select(opstina => new KatastarskaOpstina
                        {
                            Id = opstina.Id,
                            Naziv = opstina.Naziv
                        })
                        .ToList()
                };

                return View(model);
            }
            finally
            {
                _konekcija.ZatvoriKonekciju();
            }
        }
    }
}
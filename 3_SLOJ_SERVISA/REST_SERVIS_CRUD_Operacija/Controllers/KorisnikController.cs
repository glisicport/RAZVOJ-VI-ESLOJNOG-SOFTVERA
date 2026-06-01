using BibliotekaKlasa.KlasePodataka.ApiPozivi;
using BibliotekaKlasa.KlasePodatakaEF.ModeliEF;
using BibliotekaKlasa.KlasePodatakaEF.RepozitorijumiEF;
using BibliotekaKlasa.TehnoloskeKlase.PomocneFunkcije;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace REST_SERVIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KorisnikController : ControllerBase
    {
        private readonly KorisnikRepo korisnikRepo;

        public KorisnikController(KorisnikRepo repo)
        {
            korisnikRepo = repo;
        }

        [HttpPost]
        public async Task<ActionResult> Dodaj([FromBody] RegistracijaPoziv registracija)
        {
            try
            {
                var lice = new LiceModel
                {
                    Tip = registracija.Tip,
                    Ime = registracija.Ime,
                    Prezime = registracija.Prezime,
                    Jmbg = registracija.Jmbg,
                    MaticniBroj = registracija.MaticniBroj
                };

                var korisnik = new KorisnikModel
                {
                    Email = registracija.Email,
                    LozinkaHash = FunkcijeLozinke.HashujLozinku(registracija.Lozinka)
                };

                await korisnikRepo.Dodaj(korisnik, lice);

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Greska na serveru");
            }
        }
    }
}

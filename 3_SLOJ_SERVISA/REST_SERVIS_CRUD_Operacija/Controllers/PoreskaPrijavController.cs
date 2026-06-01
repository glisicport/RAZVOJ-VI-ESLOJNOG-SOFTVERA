using BibliotekaKlasa.KlasePodataka.ApiPozivi;
using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace REST_SERVIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PoreskaPrijavaController : ControllerBase
    {
        private readonly PoreskaPrijavaRepo _repozitorijum;

        public PoreskaPrijavaController(PoreskaPrijavaRepo repozitorijum)
        {
            _repozitorijum = repozitorijum;
        }

        [HttpDelete]
        public IActionResult Obrisi(Guid prijavaId)
        {
          

             try
            {
                if (prijavaId == Guid.Empty)
                    return BadRequest("prijavaId je obavezan.");

                bool uspesnoObrisano =
                _repozitorijum.Obrisi(prijavaId);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("VratiPrijavuPoId")]
        public IActionResult VratiPrijavuPoId([FromQuery] Guid prijava_id)
        {
            try
            {
                if (prijava_id == Guid.Empty)
                    return BadRequest("KorisnikId je obavezan.");

                var prijava = _repozitorijum.VratiPrijavuPoID(prijava_id);

                return Ok(prijava);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }

        }
        [HttpGet("VratiPrijavuPoKorisnikId")]
        public IActionResult VratiPrijavuPoKorisnikId([FromQuery] Guid korisnikId)
        {
            try
            {
                if (korisnikId == Guid.Empty)
                    return BadRequest("KorisnikId je obavezan.");

                var prijava = _repozitorijum.VratiPrijavuPoKorisnikID(korisnikId);

                return Ok(prijava);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult Dodaj([FromBody] PoreskaPrijavaPoziv zahtev)
        {
            try
            {
                var greske = ValidirajZahtev(zahtev);

                if (greske.Any())
                    return BadRequest(new
                    {
                        success = false,
                        errors = greske
                    });

                var prijava = KreirajPrijavu(zahtev, false);

                prijava.Nepokretnosti = KreirajNepokretnosti(zahtev.Objekti, prijava.Id);

                decimal poreskoUmanjenje =
                    PoslovnaLogika.Ogranicenja.IzracunajOslobodjenje(prijava);

                _repozitorijum.DodajPoreskuPrijavu(prijava, poreskoUmanjenje);

                return Ok(new
                {
                    success = true,
                    prijavaId = prijava.Id
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPut("Izmeni")]
        public IActionResult Izmeni(
            [FromQuery] Guid izmenaId,
            [FromBody] PoreskaPrijavaPoziv zahtev)
        {
            try
            {
                if (izmenaId == Guid.Empty)
                    return BadRequest("PrijavaId je obavezan.");

                var greske = ValidirajZahtev(zahtev);

                if (greske.Any())
                    return BadRequest(new
                    {
                        success = false,
                        errors = greske
                    });

                var prijava = KreirajPrijavu(zahtev, true);

                prijava.Id = izmenaId;

                prijava.Nepokretnosti =
                    KreirajNepokretnosti(zahtev.Objekti, izmenaId);

                decimal poreskoUmanjenje =
                    PoslovnaLogika.Ogranicenja.IzracunajOslobodjenje(prijava);

                _repozitorijum.IzmeniPoreskaPrijava(prijava, poreskoUmanjenje);

                return Ok(new
                {
                    success = true,
                    prijavaId = prijava.Id
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        private PoreskaPrijavaModel KreirajPrijavu(
            PoreskaPrijavaPoziv zahtev,
            bool izmena)
        {
            var trenutniKorisnikId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var prijavaId = Guid.NewGuid();

            var prijava = new PoreskaPrijavaModel
            {
                Id = prijavaId,

                VrstaPravaId = zahtev.VrstaPravaId!.Value,
                JlsId = zahtev.JlsId!.Value,

                ImaPoreskiKredit =
                    zahtev.PoreskiPodaci?.ImaKredit ?? false,

                ImaPoreskoOslobodjenje =
                    zahtev.PoreskiPodaci?.ImaOslobodjenje ?? false,

                KreditOsnov =
                    zahtev.PoreskiPodaci?.Osnova?.ToString(),

                OrganizacionaJedinica =
                    zahtev.OrganizacionaJedinica ?? "",

                DanNastanka =
                    zahtev.PoreskiPodaci?.DatumNastanka,

                DanPrestanka =
                    zahtev.PoreskiPodaci?.DatumPrestanka,
                oslobodjenje_osnov = zahtev.PoreskiPodaci?.oslobodjenje_osnov,
                kredit_osnov = zahtev.PoreskiPodaci?.kredit_osnov,
                PopisDokaza =
                    zahtev.PoreskiPodaci?.Dokazi,

                Napomena3 =
                    zahtev.PoreskiPodaci?.Napomena,

                DatumPodnosenja = DateTime.Now,
                KreiranaU = DateTime.Now,
                AzuriranaU = DateTime.Now,

                JeIzmenjena = izmena,

                ObveznikLiceId =
                    zahtev.Obveznik?.Id ??
                    Guid.Parse(trenutniKorisnikId!),

                PodnosilacLiceId =
                    !string.IsNullOrWhiteSpace(zahtev.Podnosilac)
                        ? Guid.Parse(zahtev.Podnosilac)
                        : zahtev.Obveznik?.Id ??
                          Guid.Parse(trenutniKorisnikId!)
            };

            return prijava;
        }

        private List<Nepokretnost> KreirajNepokretnosti(
            List<NepokretnostDto>? objekti,
            Guid prijavaId)
        {
            var nepokretnosti = new List<Nepokretnost>();

            if (objekti == null)
                return nepokretnosti;

            int redniBroj = 1;

            foreach (var objekat in objekti)
            {
                nepokretnosti.Add(new Nepokretnost
                {
                    Id = Guid.NewGuid(),
                    PrijavaId = prijavaId,

                    Rbr = redniBroj++,

                    VrstaId = objekat.VrstaId ?? -1,

                    KatastarskaOpstinaId =
                        objekat.KatastarskaOpstinaId,

                    UlicaIbroj =
                        objekat.ulicaIbroj,

                    BrKatParcele =
                        objekat.BrKatParcele,

                    GodinaIzgradnje =
                        objekat.GodinaIzgradnje,

                    KorisnaPovrsina =
                        objekat.KorisnaPovrsina,

                    UkupnaPovrsina =
                        objekat.UkupnaPovrsina,

                    UdeoObveznika =
                        objekat.UdeoObveznika,


                    ImaPoreskiKredit =
                        objekat.ImaPoreskiKredit,

                    ImaPoreskoOslobodjenje =
                        objekat.ImaPoreskoOslobodjenje,

                    OslobodjenjeOsnov =
                        objekat.OslobodjenjeOsnov,

                    Napomena =
                        objekat.Napomena ?? ""
                });
            }

            return nepokretnosti;
        }

        private List<string> ValidirajZahtev(PoreskaPrijavaPoziv zahtev)
        {
            var greske = new List<string>();

            if (zahtev == null)
            {
                greske.Add("Zahtev je prazan.");
                return greske;
            }

            if (zahtev.VrstaPravaId == null)
                greske.Add("VrstaPravaId je obavezan.");

            if (zahtev.JlsId == null || zahtev.JlsId == Guid.Empty)
                greske.Add("JlsId je obavezan.");

            if (!string.IsNullOrWhiteSpace(zahtev.Podnosilac) &&
                !Guid.TryParse(zahtev.Podnosilac, out _))
            {
                greske.Add("Podnosilac mora biti validan GUID.");
            }

            ValidirajPoreskePodatke(zahtev, greske);
            ValidirajObjekte(zahtev.Objekti, greske);

            return greske;
        }

        private void ValidirajPoreskePodatke(
            PoreskaPrijavaPoziv zahtev,
            List<string> greske)
        {
            var podaci = zahtev.PoreskiPodaci;

            if (podaci == null)
                return;

          

            if (podaci.ImaOslobodjenje &&
                podaci.oslobodjenje_osnov == null)
            {
                greske.Add(
                    "Oslobodjenje je obavezno kada postoji oslobodjenje.");
            }
            if (podaci.ImaKredit &&
                podaci.kredit_osnov == null)
            {
                greske.Add(
                    "Poreska osnova je obavezna kada postoji kredit.");
            }
        }

        private void ValidirajObjekte(
            List<NepokretnostDto>? objekti,
            List<string> greske)
        {
            if (objekti == null)
                return;

            for (int i = 0; i < objekti.Count; i++)
            {
                var objekat = objekti[i];
                var indeks = i + 1;

                if (objekat == null)
                {
                    greske.Add($"Objekat[{indeks}] je null.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(objekat.ulicaIbroj))
                    greske.Add($"Objekat[{indeks}] nema adresu.");

                if (objekat.KorisnaPovrsina <= 0)
                    greske.Add($"Objekat[{indeks}] nema validnu korisnu povrsinu.");

                if (objekat.UkupnaPovrsina <= 0)
                    greske.Add($"Objekat[{indeks}] nema validnu ukupnu povrsinu.");

                if (objekat.UkupnaPovrsina < objekat.KorisnaPovrsina)
                    greske.Add($"Objekat[{indeks}] ukupna povrsina ne moze biti manja od korisne.");

                if (objekat.UdeoObveznika < 0 ||
                    objekat.UdeoObveznika > 100)
                {
                    greske.Add($"Objekat[{indeks}] nema validan udeo.");
                }

                if (objekat.KatastarskaOpstinaId == Guid.Empty)
                    greske.Add($"Objekat[{indeks}] nema validnu katastarsku opstinu.");

                if (objekat.VrstaId == null)
                    greske.Add($"Objekat[{indeks}] nema vrstu.");

                
            }
        }
    }
}
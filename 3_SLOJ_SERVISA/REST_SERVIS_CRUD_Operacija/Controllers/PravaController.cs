using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using Microsoft.AspNetCore.Mvc;

namespace REST_SERVIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PravaController : ControllerBase
    {
        private readonly VrstaPravaRepozitorijum _repo;

        public PravaController(VrstaPravaRepozitorijum repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public ActionResult<List<VrstaPravaModel>> Get()
        {
            var rezultat = _repo.DajSvaPrava();
            return Ok(rezultat);
        }
    }
}
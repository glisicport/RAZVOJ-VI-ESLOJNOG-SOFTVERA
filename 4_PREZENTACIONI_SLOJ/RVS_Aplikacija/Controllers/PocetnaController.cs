using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RVS_Aplikacija.ViewModels;

namespace RVS_Aplikacija.Controllers
{
    public class PocetnaController : Controller
    {
        private readonly ILogger<PocetnaController> _logger;

        public PocetnaController(ILogger<PocetnaController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privatnost()
        {
            return View();   
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new GreskaViewModel { IdPoziva = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

  
    }
}

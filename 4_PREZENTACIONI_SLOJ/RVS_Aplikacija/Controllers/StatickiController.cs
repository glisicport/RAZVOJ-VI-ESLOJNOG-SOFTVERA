using Microsoft.AspNetCore.Mvc;

namespace RVS_Aplikacija.Controllers
{
    public class StatickiController : Controller
    {
        public IActionResult OSistemu()
        {
            return View();
        }
        public IActionResult Svrha()
        {
            return View();
        }
    }
}

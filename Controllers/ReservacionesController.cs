using Microsoft.AspNetCore.Mvc;

namespace APP_HotelBeachSA.Controllers
{
    public class ReservacionesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Reservar()
        {
            return View();
        }
    }
}

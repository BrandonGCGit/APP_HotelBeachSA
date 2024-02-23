using APP_HotelBeachSA.Models;
using Microsoft.AspNetCore.Mvc;

namespace APP_HotelBeachSA.Controllers
{
    public class ClientesController : Controller
    {
        private HotelBeachAPI hotelBeachAPI;
        private HttpClient client;

        public ClientesController()
        {
            hotelBeachAPI = new HotelBeachAPI();
            client = hotelBeachAPI.Inicial();
        }

        [HttpGet]
        public IActionResult CheckId()
        {
            return View();
        }


        [HttpPost]
        public IActionResult CheckId(string cedula)
        {

            if (!string.IsNullOrWhiteSpace(cedula))
            {
                // Aquí se llamaría al API para verificar si la cédula existe
                var clienteExiste = false;

                if (!clienteExiste)
                {
                    // Si la cédula no existe, redirige directamente al formulario de registro
                    return RedirectToAction("Register", new { cedula });
                }
                else
                {
                    // Si la cédula existe, redirige a la sección de reservación
                    return RedirectToAction("Reservation", new { cedula });
                }
            }
            else
            {
                // Si el ID es null o vacío, redirige al usuario de vuelta al formulario de CheckCedula con el msj de error
                TempData["Mensaje"] = "Error registering account.";
                return RedirectToAction("CheckId");
            }
        }

        [HttpGet]
        public IActionResult Register(string cedula, string name)
        {
            var model = new Cliente { Cedula = cedula, Nombre = "Nombre del cliente" };

            return View(model);
        }

    }
}

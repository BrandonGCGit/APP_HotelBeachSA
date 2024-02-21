using APP_HotelBeachSA.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APP_HotelBeachSA.Controllers
{
    public class ReservacionesController : Controller
    {
        private HotelBeachAPI hotelBeachAPI;

        //Variable para manejar la referencia del object  HttpcClient
        private HttpClient httpClient;

        private static int idPaquete;

        public ReservacionesController()
        {
            hotelBeachAPI = new HotelBeachAPI();
            httpClient = hotelBeachAPI.Inicial();
        }

        public async Task<IActionResult> Index()
        {
            //lista de libros
            List<Paquete> listado = new List<Paquete>();

            //Se utliza el método de la API
            HttpResponseMessage response = await httpClient.GetAsync("api/Paquetes/Listado");

            //Si todo fue correcto
            if (response.IsSuccessStatusCode)
            {
                //Se realiza la lectura datos
                var resultados = response.Content.ReadAsStringAsync().Result;

                //Se toman los datos en JSON se convierte en un listado de objecto
                listado = JsonConvert.DeserializeObject<List<Paquete>>(resultados);
            }
            return View(listado);
        }

        [HttpPost]
        public IActionResult Index([Bind] Paquete paquete)
        {
            return View(paquete);
        }

        [HttpGet]
        public IActionResult CrearReservacion(int id)
        {
            idPaquete = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearReservacion([Bind] SuperReservacion superReservacion)
        {
            // Almacenar el objeto superReservacion en TempData
            TempData["SuperReservacion"] = JsonConvert.SerializeObject(superReservacion);
            return RedirectToAction("Confirmacion", "Reservaciones");
        }

        [HttpGet]
        public async Task<IActionResult> Confirmacion()
        {
            // Recuperar el objeto superReservacion desde TempData
            string superReservacionJson = TempData["SuperReservacion"] as string;
            SuperReservacion superReservacion = JsonConvert.DeserializeObject<SuperReservacion>(superReservacionJson);

            HttpResponseMessage response = await httpClient.GetAsync($"api/Paquetes/Constultar?id={idPaquete}");
            if (response.IsSuccessStatusCode)
            {
                var paquete = response.Content.ReadAsStringAsync().Result;

                //Se convierte el JSON en un Object
                superReservacion.Paquete = JsonConvert.DeserializeObject<Paquete>(paquete);
            }
            return View(superReservacion);
        }

    }
}

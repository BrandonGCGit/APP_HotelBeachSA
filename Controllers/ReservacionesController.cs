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

        public IActionResult Reservar()
        {
            return View();
        }
    }
}

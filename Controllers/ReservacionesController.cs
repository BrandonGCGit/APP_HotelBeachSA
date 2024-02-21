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

            HttpResponseMessage response = await httpClient.GetAsync($"api/Paquetes/Consultar?id={idPaquete}");
            if (response.IsSuccessStatusCode)
            {
                var paquete = response.Content.ReadAsStringAsync().Result;

                //Se convierte el JSON en un Object
                superReservacion.Paquete = JsonConvert.DeserializeObject<Paquete>(paquete);
            }


            //Calculo de Noches
            // Calcula la diferencia de días entre las fechas de entrada y salida
            TimeSpan diferencia = superReservacion.Reservacion.Salida.Subtract(superReservacion.Reservacion.Entrada);

            // Obtiene la cantidad de noches redondeando hacia arriba
            int cantidadNoches = (int)Math.Ceiling(diferencia.TotalDays);

            if(cantidadNoches >= 3 && cantidadNoches <= 6)
            {
                /// total = (noches * precioPaquete) / descuento
            }
            else if(cantidadNoches >= 7 &&  cantidadNoches <= 9)
            {
                ////
            }
            else if(cantidadNoches >= 10 && cantidadNoches <= 12)
            {
                ///
            }
            else if(cantidadNoches >= 13)
            {
                ///
            }
            else
            {
                /// total = noche * paquetes
            }

            //Para llenar los campos de PAGO

            superReservacion.Pago.Fecha_Registro = DateTime.Now;
            switch (superReservacion.Pago.Tipo_Pago)
            {
                case 'K':
                    superReservacion.Pago.Numero_Pago = 0;
                    break;
                case 'T':                    
                    break;
                case 'C':
                    superReservacion.Cheque.Id = superReservacion.Pago.Numero_Pago;
                    superReservacion.Cheque.Estado = 'A';
                    break;
            }



            return View(superReservacion);
        }

    }
}

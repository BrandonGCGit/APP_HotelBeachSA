using APP_HotelBeachSA.Models;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

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
        }//constructo

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
        }//Index

        [HttpPost]
        public IActionResult Index([Bind] Paquete paquete)
        {
            return View(paquete);
        }//Index

        [HttpGet]
        public IActionResult CrearReservacion(int id)
        {
            idPaquete = id;
            return View();
        }//CrearReservacion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearReservacion([Bind] SuperReservacion superReservacion)
        {
            // Almacenar el objeto superReservacion en TempData
            TempData["SuperReservacion"] = JsonConvert.SerializeObject(superReservacion);
            return RedirectToAction("Confirmacion", "Reservaciones");
        }//CrearReservacion

        [HttpGet]
        public async Task<IActionResult> Confirmacion()
        {
            // Recuperar el objeto superReservacion desde TempData
            string superReservacionJson = TempData["SuperReservacion"] as string;
            SuperReservacion superReservacion = JsonConvert.DeserializeObject<SuperReservacion>(superReservacionJson);


            ///Optimizar
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
            superReservacion.Reservacion.Noches = cantidadNoches;

            await CalcularDescuento(cantidadNoches, superReservacion);
          

            ///Cálculo total de Reservaciones
            superReservacion.CostoPersona = Decimal.Multiply(superReservacion.Reservacion.Noches, superReservacion.Paquete.Costo_Persona);
            var totalReservacion = Decimal.Multiply(superReservacion.CostoPersona, superReservacion.Reservacion.Huespedes);
            decimal porcentajeDecimal = superReservacion.Discount.Porcentaje / 100m;
            superReservacion.MontoDescuento = Decimal.Multiply(totalReservacion, porcentajeDecimal);
            superReservacion.Iva = decimal.Multiply(totalReservacion, 0.13m);
            superReservacion.Reservacion.Total = (totalReservacion + superReservacion.Iva) - superReservacion.MontoDescuento;

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
            }//Fin del switch

            return View(superReservacion);
        }//Confirmacion


        /// <summary>
        /// Método para determinar el tipo de descuento
        /// </summary>
        /// <param name="cantidadNoches"></param>
        /// <param name="superReservacion"></param>
        private async Task<bool> CalcularDescuento(int cantidadNoches, SuperReservacion superReservacion)
        {

            if (cantidadNoches >= 3 && cantidadNoches <= 6)
            {
                /// total = (noches * precioPaquete) / descuento
                HttpResponseMessage respuestaDescuento = await httpClient.GetAsync($"api/Descuentos/Consultar?id={1}");
                if (respuestaDescuento.IsSuccessStatusCode)
                {
                    var descuento = respuestaDescuento.Content.ReadAsStringAsync().Result;

                    //Se convierte el JSON en un Object
                    superReservacion.Discount = JsonConvert.DeserializeObject<Discount>(descuento);
                    return true;
                }//
                
            }
            else if (cantidadNoches >= 7 && cantidadNoches <= 9)
            {
                HttpResponseMessage respuestaDescuento = await httpClient.GetAsync($"api/Descuentos/Consultar?id={2}");
                if (respuestaDescuento.IsSuccessStatusCode)
                {
                    var descuento = respuestaDescuento.Content.ReadAsStringAsync().Result;

                    //Se convierte el JSON en un Object
                    superReservacion.Discount = JsonConvert.DeserializeObject<Discount>(descuento);
                    return true;
                }//
            }
            else if (cantidadNoches >= 10 && cantidadNoches <= 12)
            {
                ///
                HttpResponseMessage respuestaDescuento = await httpClient.GetAsync($"api/Descuentos/Consultar?id={3}");
                if (respuestaDescuento.IsSuccessStatusCode)
                {
                    var descuento = respuestaDescuento.Content.ReadAsStringAsync().Result;

                    //Se convierte el JSON en un Object
                    superReservacion.Discount = JsonConvert.DeserializeObject<Discount>(descuento);
                    return true;
                }//
            }
            else if (cantidadNoches >= 13)
            {
                ///
                HttpResponseMessage respuestaDescuento = await httpClient.GetAsync($"api/Descuentos/Consultar?id={4}");
                if (respuestaDescuento.IsSuccessStatusCode)
                {
                    var descuento = respuestaDescuento.Content.ReadAsStringAsync().Result;

                    //Se convierte el JSON en un Object
                    superReservacion.Discount = JsonConvert.DeserializeObject<Discount>(descuento);
                    return true;
                }//
            }
            else
            {
                /// total = noche * paquetes
                HttpResponseMessage respuestaDescuento = await httpClient.GetAsync($"api/Descuentos/Consultar?id={5}");
                if (respuestaDescuento.IsSuccessStatusCode)
                {
                    var descuento = respuestaDescuento.Content.ReadAsStringAsync().Result;

                    //Se convierte el JSON en un Object
                    superReservacion.Discount = JsonConvert.DeserializeObject<Discount>(descuento);
                    return true;
                }//
            }

            return false;
        }//CalcularDescuento


    }//Class

  


   
}//namespace

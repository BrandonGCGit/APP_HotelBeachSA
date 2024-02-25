using APP_HotelBeachSA.Model;
using APP_HotelBeachSA.Models;
using Azure;
using iText.Kernel.Pdf.Canvas.Wmf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System.Net.Http;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace APP_HotelBeachSA.Controllers
{
    public class ReservacionesController : Controller
    {
        private HotelBeachAPI hotelBeachAPI;

        private TipoCambioAPI tipoCambioAPI;

        //Variable para manejar la referencia del object  HttpcClient
        private HttpClient httpClient;

        private HttpClient clientTipoCambio;

        //variable para almacenar el valor del tipo de cambio
        public static TipoCambio tipoCambio;

        private static int idPaquete;

        private SuperReservacion superReservacion = null;

        public ReservacionesController()
        {
            hotelBeachAPI = new HotelBeachAPI();

            httpClient = hotelBeachAPI.Inicial();

            tipoCambioAPI = new TipoCambioAPI();

            clientTipoCambio = tipoCambioAPI.Inicial();

            //Se llama al método para extraer tipo de cambio
            extraerTipoCambio();
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
            superReservacion = JsonConvert.DeserializeObject<SuperReservacion>(superReservacionJson);


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
            if (cantidadNoches <= 0)
            {
                TempData["Mensaje"] = "Fallo con la fecha definida. Por favor intente de nuevo";
                superReservacion = null;
                return RedirectToAction("RegistrarDB", "Reservaciones");
            }
            superReservacion.Reservacion.Noches = cantidadNoches;


            await CalcularDescuento(cantidadNoches);

            await CalcularMontoReservacion();


            TempData["SuperReservacion"] = JsonConvert.SerializeObject(superReservacion);
            return View(superReservacion);
        }//Confirmacion



        [HttpGet]
        public async Task<IActionResult> RegistrarDB()
        {
            if (superReservacion != null)
            {
                string superReservacionJson = TempData["SuperReservacion"] as string;
                superReservacion = JsonConvert.DeserializeObject<SuperReservacion>(superReservacionJson);


                if (await RegistrarPago())
                {
                    return View();
                }
                else
                {
                    TempData["Mensaje"] = "Fallo con el registro de la Reservación";
                    return View();
                }
            }
            else
            {
                return View();
            }

        }


        //------------------------METODOS AUXILIARES----------------------------
        /// <summary>
        /// Método para determinar el tipo de descuento
        /// </summary>
        /// <param name="cantidadNoches"></param>
        /// <param name="superReservacion"></param>
        private async Task<bool> CalcularDescuento(int cantidadNoches)
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

        /// <summary>
        /// Método para Calcular el Monto Final de la Reservacion
        /// </summary>
        /// <param name="superReservacion"></param>
        /// <returns></returns>
        private async Task<bool> CalcularMontoReservacion()
        {
            try
            {
                ///Cálculo total de Reservaciones

                //Cálculo de Costo por persona
                superReservacion.CostoPersona = Decimal.Multiply(superReservacion.Reservacion.Noches, superReservacion.Paquete.Costo_Persona);

                //Cálculo de Monto Total
                superReservacion.CostoTotal = Decimal.Multiply(superReservacion.CostoPersona, superReservacion.Reservacion.Huespedes);

                //Costo de procentaje de descuento
                decimal porcentajeDecimal = superReservacion.Discount.Porcentaje / 100m;
                superReservacion.MontoDescuento = Decimal.Multiply(superReservacion.CostoTotal, porcentajeDecimal);

                //Cálcula del porcentaje de IVA
                superReservacion.Iva = Decimal.Multiply(superReservacion.CostoTotal, 0.13m);
                decimal porcentajeAdelanto = superReservacion.Paquete.Adelanto / 100m;

                //Costo del Monto Total + IVA - Monto de Descuento
                superReservacion.Reservacion.Total = (superReservacion.CostoTotal + superReservacion.Iva) - superReservacion.MontoDescuento;

                superReservacion.MontoColones = Decimal.Multiply(superReservacion.Reservacion.Total, tipoCambio.venta);

                //Monto del adelanto
                superReservacion.Adelanto = Decimal.Multiply(superReservacion.Reservacion.Total, porcentajeAdelanto);


                //Datos sobre el pago
                superReservacion.Pago.Fecha_Registro = DateTime.Now;
                switch (superReservacion.Pago.Tipo_Pago)
                {
                    //Cash
                    case 'K':
                        Random rnd = new Random();
                        int numeroAleatorio = rnd.Next(10000000, 99999999);

                        superReservacion.Pago.Numero_Pago = numeroAleatorio;
                        superReservacion.TipoPago = "Cash";
                        break;
                    //Card
                    case 'T':
                        superReservacion.TipoPago = "Card";
                        break;
                    //Check
                    case 'C':
                        superReservacion.Cheque.Id = superReservacion.Pago.Numero_Pago;
                        superReservacion.TipoPago = "Check";
                        break;
                }//Fin del switch


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }//Try           

        }//CalcularMontoReservacion

        /// <summary>
        /// Método para Registrar datos en la bd
        /// </summary>
        /// <returns></returns>
        private async Task<bool> RegistrarCheque()
        {
            if (superReservacion.Pago.Tipo_Pago.Equals('C'))
            {
                superReservacion.Cheque.Id = superReservacion.Pago.Id;
                superReservacion.Cheque.Estado = 'A';
                var agregar = httpClient.PostAsJsonAsync<Cheque>("api/Cheques/Agregar", superReservacion.Cheque);

                await agregar;

                var resultado = agregar.Result;

                if (resultado.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    TempData["Mensaje"] = "Fallo con el registro del cheque. Por favor intente de nuevo";
                    return false;
                }
            }
            else
            {
                return true;
            }

        }

        private async Task<bool> RegistrarPago()
        {
            if (await RegistrarReservacion() > 0)
            {
                superReservacion.Pago.Id_Cliente = "208140785";
                superReservacion.Pago.Id_Reservacion = superReservacion.Reservacion.Id;
                HttpResponseMessage response = await httpClient.PostAsJsonAsync<Pago>("api/Pagos/Agregar", superReservacion.Pago);

                if (response.IsSuccessStatusCode)
                {
                    var resultados = response.Content.ReadAsStringAsync().Result;

                    superReservacion.Pago = JsonConvert.DeserializeObject<Pago>(resultados);

                    if (superReservacion.Pago.Id > 0)
                    {
                        if (await RegistrarCheque())
                        {
                            if (EnviarEmail(superReservacion))
                            {
                                TempData["Mensaje"] = "Reservación registrada con éxito. Los detalles fueron enviados a su correo";
                                return true;
                            }
                            else
                            {
                                TempData["Mensaje"] = "Reservación registrada con éxito. Fallo con el envío de Email. Contacte nuestro equipo de soporte";
                                return false;
                            }
                        }
                        else
                        {
                            HttpResponseMessage eliminarPago = await httpClient.DeleteAsync($"/api/Roles/Eliminar?id={superReservacion.Pago.Id}");
                            HttpResponseMessage eliminarReservacion = await httpClient.DeleteAsync($"/api/Roles/Eliminar?id={superReservacion.Reservacion.Id}");
                            return false;
                        }
                    }
                    else
                    {
                        TempData["Mensaje"] = "Fallo con el registro del pago. Por favor intente de nuevo";
                        return false;
                    }



                }
                else
                {
                    TempData["Mensaje"] = "Fallo con el registro de la reservación. Por favor intente de nuevo";
                    return false;
                }
            }
            else
            {
                return false;
            }


        }

        private async Task<int> RegistrarReservacion()
        {
            if (superReservacion != null)
            {
                superReservacion.Reservacion.Id_Cliente = "208140785";
                superReservacion.Reservacion.Id_Paquete = superReservacion.Paquete.Id;
                superReservacion.Reservacion.Id_Descuento = superReservacion.Discount.Id;
                superReservacion.Reservacion.Fecha_Registro = DateTime.Now;

                HttpResponseMessage response = await httpClient.PostAsJsonAsync<Reservacion>("api/Reservaciones/Agregar", superReservacion.Reservacion);

                Reservacion reservacion = new Reservacion();
                if (response.IsSuccessStatusCode)
                {
                    var resultados = response.Content.ReadAsStringAsync().Result;

                    superReservacion.Reservacion = JsonConvert.DeserializeObject<Reservacion>(resultados);

                    return superReservacion.Reservacion.Id;
                }
                else
                {
                    TempData["Mensaje"] = "No se logro registrar el reservacion...";
                    return 0;
                }
            }
            else
            {
                TempData["Mensaje"] = "Error con la reservación...";
                return 0;
            }

        }//registrar reservacion

        /// <summary>
        /// Método para extraer el tipo de cambio
        /// </summary>
        [HttpGet]
        private async void extraerTipoCambio()
        {
            try
            {
                //Se consume el método para la API
                HttpResponseMessage response = await clientTipoCambio.GetAsync("tdc/tdc.json");

                //Se valida si todo fue correcto
                if (response.IsSuccessStatusCode)
                {
                    //Se realiza lectura de los datos en formato JSON
                    var result = response.Content.ReadAsStringAsync().Result;

                    //Se convierte el JSON en un Objeto TipoCambio
                    tipoCambio = JsonConvert.DeserializeObject<TipoCambio>(result);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Método para el envío de email
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        private bool EnviarEmail(SuperReservacion superReservacion)
        {
            try
            {
                //Variable control
                bool enviado = false;

                //Se instacia el objeto email
                Email email = new Email();

                //Se utiliza el metodo para enviar el email
                email.Enviar(superReservacion);

                //se indica yupii se envió el email
                enviado = true;

                return enviado;
            }
            catch (Exception ex)
            {
                return false;
            }
        }//cierre Enviar Email

    }//Class





}//namespace

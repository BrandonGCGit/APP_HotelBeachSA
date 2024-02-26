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

        private GometaAPI gometaAPI;

        //Variable para manejar la referencia del object  HttpcClient
        private HttpClient httpClient;

        private HttpClient clientGometa;

        //variable para almacenar el valor del tipo de cambio
        public static TipoCambio tipoCambio;

        //variable para almacenar los datos por cedula
        public ClienteGometa clienteGometa;

        private static int idPaquete;
        private static string idCliente;

        private SuperReservacion superReservacion = new SuperReservacion();

        public ReservacionesController()
        {
            hotelBeachAPI = new HotelBeachAPI();

            httpClient = hotelBeachAPI.Inicial();

            gometaAPI = new GometaAPI();

            clientGometa = gometaAPI.Inicial();
        }//constructor


        /// <summary>
        /// Método para mostrar los paquetes registrados
        /// </summary>
        /// <returns></returns>
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

        [HttpGet]
        public async Task<IActionResult> BuscarCliente(int id)
        {
            idPaquete = id;
            return View();
        }//BuscarCliente

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuscarCliente(string cedula)
        {
            Cliente pCliente = new Cliente();
            HttpResponseMessage respuesta = await httpClient.GetAsync($"/api/Clientes/Consultar?cedula={cedula}");

            if (respuesta.IsSuccessStatusCode)
            {
                var resultado = respuesta.Content.ReadAsStringAsync().Result;

                pCliente = JsonConvert.DeserializeObject<Cliente>(resultado);
            }
            else
            {
                pCliente = null;
            }


            //Si el cliente ya se ha registrado
            if (pCliente != null)
            {
                idCliente = cedula;
                superReservacion.Cliente = pCliente;
                TempData["SuperReservacion"] = JsonConvert.SerializeObject(superReservacion);
                return RedirectToAction("CrearReservacion", "Reservaciones");
            }
            else
            {
                //Extraer los datos segun la cedula
                if (await extraerDatosCedula(cedula))
                {
                    //Crear objeto cliente con la informacion
                    Cliente nuevoCliente = new Cliente();
                    nuevoCliente.Cedula = clienteGometa.cedula;
                    nuevoCliente.Tipo_Cedula = clienteGometa.guess_type;
                    nuevoCliente.Nombre = clienteGometa.firstname;
                    nuevoCliente.Primer_Apellido = clienteGometa.lastname1;
                    nuevoCliente.Segundo_Apellido = clienteGometa.lastname2;

                    //Almacenar superReservacion con los datos
                    superReservacion.Cliente = nuevoCliente;
                    TempData["SuperReservacion"] = JsonConvert.SerializeObject(superReservacion);

                    //Redirigir a RegistrarCliente
                    return RedirectToAction("RegistrarCliente", "Reservaciones");
                }
                else
                {
                    TempData["MensajeCedula"] = "No se logró encontrar la cédula";
                    return View();
                }
            }
            //return View(pCliente);
        }//BuscarCliente

        /// <summary>
        /// Método para mostrar el form de RegistrarCliente
        /// </summary>
        /// <param name="pCliente"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RegistrarCliente()
        {
            // Recuperar el objeto superReservacion desde TempData
            string superReservacionJson = TempData["SuperReservacion"] as string;

            //Se asigna el objeto
            superReservacion = JsonConvert.DeserializeObject<SuperReservacion>(superReservacionJson);

            return View(superReservacion.Cliente);
        }//RegistrarCliente

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarCliente([Bind] Cliente pCliente)
        {
            pCliente.Fecha_Registro = DateTime.Now;
            if (ModelState.IsValid)
            {

                HttpResponseMessage response = await httpClient.PostAsJsonAsync<Cliente>("/api/Clientes/Crear", pCliente);

                //Reservacion reservacion = new Reservacion();
                if (response.IsSuccessStatusCode)
                {
                    var resultados = response.Content.ReadAsStringAsync().Result;

                    superReservacion.Cliente = JsonConvert.DeserializeObject<Cliente>(resultados);
                    idCliente = superReservacion.Cliente.Cedula;
                    TempData["SuperReservacion"] = JsonConvert.SerializeObject(superReservacion);
                    return RedirectToAction("CrearReservacion", "Reservaciones");
                }
                else
                {
                    TempData["MensajeCliente"] = "No se logro registrar el cliente...";
                    return View(pCliente);
                }
            }
            else
            {
                return View(pCliente);
            }


        }//RegistrarCliente

        /// <summary>
        /// Método para mostrar las vista de las fechas y los métodos de pago
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CrearReservacion()
        {
            // Recuperar el objeto superReservacion desde TempData
            string superReservacionJson = TempData["SuperReservacion"] as string;

            //Se asigna el objeto
            superReservacion = JsonConvert.DeserializeObject<SuperReservacion>(superReservacionJson);
            return View(superReservacion);
        }//CrearReservacion




        /// <summary>
        /// Método para enviar guardar temporalmente los datos de la superReservación
        /// </summary>
        /// <param name="superReservacion"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearReservacion([Bind] SuperReservacion superReservacion)
        {

           

            ///Recuperar el id del paquete para almacenar la info del Paquete
            HttpResponseMessage response = await httpClient.GetAsync($"api/Paquetes/Consultar?id={idPaquete}");
            if (response.IsSuccessStatusCode)
            {
                var paquete = response.Content.ReadAsStringAsync().Result;

                //Se convierte el JSON en un Object
                superReservacion.Paquete = JsonConvert.DeserializeObject<Paquete>(paquete);
            }

            //Recuperar el id del Cliente para almacenar en superReservacion
            HttpResponseMessage responseCliente = await httpClient.GetAsync($"/api/Clientes/Consultar?cedula={idCliente}");
            if (responseCliente.IsSuccessStatusCode)
            {
                var cliente = responseCliente.Content.ReadAsStringAsync().Result;

                //Se convierte el JSON en un Object
                superReservacion.Cliente = JsonConvert.DeserializeObject<Cliente>(cliente);
            }


            //Validación de fechas
            // Verificar si la fecha de check-in es menor o igual a la fecha actual
            if (superReservacion.Reservacion.Entrada.Date <= DateTime.Now.Date)
            {
                TempData["MensajeReserva"] = "La fecha de check-in debe ser posterior a la fecha actual.";
                TempData["SuperReservacion"] = JsonConvert.SerializeObject(superReservacion);
                return View(superReservacion);
            }

            // Verificar si la fecha de salida es menor que la fecha de entrada
            if (superReservacion.Reservacion.Salida <= superReservacion.Reservacion.Entrada)
            {
                TempData["MensajeReserva"] = "La fecha de salida debe ser posterior a la fecha de check-in.";
                TempData["SuperReservacion"] = JsonConvert.SerializeObject(superReservacion);
                return View(superReservacion);
            }
                      

            // Calcula la diferencia de días entre las fechas de entrada y salida
            TimeSpan diferencia = superReservacion.Reservacion.Salida.Subtract(superReservacion.Reservacion.Entrada);
            // Obtiene la cantidad de noches redondeando hacia arriba
            int cantidadNoches = (int)Math.Ceiling(diferencia.TotalDays);            
            superReservacion.Reservacion.Noches = cantidadNoches;


            // Almacenar el objeto superReservacion en TempData
            TempData["SuperReservacion"] = JsonConvert.SerializeObject(superReservacion);

            //Redirigir a la confirmacion
            return RedirectToAction("Confirmacion", "Reservaciones");
        }//CrearReservacion


        /// <summary>
        /// Método para llenar los datos restantes de la superReservación
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Confirmacion()
        {
            // Recuperar el objeto superReservacion desde TempData
            string superReservacionJson = TempData["SuperReservacion"] as string;

            //Se asigna el objeto
            superReservacion = JsonConvert.DeserializeObject<SuperReservacion>(superReservacionJson);



            //Esperamos a que se ejecute el método para el cálculo de los montos de la reservación
            await CalcularMontoReservacion();

            //Almacenamos de manera temporal el Modelo SuperReservacion
            TempData["SuperReservacion"] = JsonConvert.SerializeObject(superReservacion);

            //Se devuelve la vista con los datos
            return View(superReservacion);
        }//Confirmacion


        /// <summary>
        /// Método para registrar los modelos en la base de datos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RegistrarDB()
        {
            //Se recupera el objeto almacenado temporalmente
            string superReservacionJson = TempData["SuperReservacion"] as string;
            superReservacion = JsonConvert.DeserializeObject<SuperReservacion>(superReservacionJson);

            //si es diferente de null
            if (superReservacion != null)
            {
                //Esperamos a que registre los datos del pago
                if (await RegistrarPago())
                {
                    //Retornamos una vista con mensaje
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
                TempData["Mensaje"] = "Fallo con el registro de la Reservación";
                return View();
            }

        }//RegistrarDB


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
                superReservacion.Discount = await GetDiscount(1);
                return true;
                
            }
            else if (cantidadNoches >= 7 && cantidadNoches <= 9)
            {
                superReservacion.Discount = await GetDiscount(2);
                return true;
            }
            else if (cantidadNoches >= 10 && cantidadNoches <= 12)
            {
                superReservacion.Discount = await GetDiscount(3);
                return true;
            }
            else if (cantidadNoches >= 13)
            {
                superReservacion.Discount = await GetDiscount(4);
                return true;
            }
            else
            {
                superReservacion.Discount = await GetDiscount(5);
                return true;
            }
            
        }//CalcularDescuento


        private async Task<Discount> GetDiscount(int id)
        {
            /// total = (noches * precioPaquete) / descuento
            HttpResponseMessage respuestaDescuento = await httpClient.GetAsync($"api/Descuentos/Consultar?id={id}");
            if (respuestaDescuento.IsSuccessStatusCode)
            {
                var descuento = respuestaDescuento.Content.ReadAsStringAsync().Result;

                //Se convierte el JSON en un Object
                superReservacion.Discount = JsonConvert.DeserializeObject<Discount>(descuento);
                return superReservacion.Discount;
            }//
            return null;
        }

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
                /////Datos sobre el pago
                superReservacion.Pago.Fecha_Registro = DateTime.Now;
                switch (superReservacion.Pago.Tipo_Pago)
                {
                    //Cash
                    case 'K':
                        Random rnd = new Random();
                        int numeroAleatorio = rnd.Next(10000000, 99999999);

                        //Esperamos a que se ejecute el metodo para calcular la cantidad de noches
                        await CalcularDescuento(superReservacion.Reservacion.Noches);

                        superReservacion.Pago.Numero_Pago = numeroAleatorio;
                        superReservacion.TipoPago = "Cash";
                        break;
                    //Card
                    case 'T':
                        superReservacion.Discount = await GetDiscount(5);
                        superReservacion.Pago.Numero_Pago = superReservacion.CardNumber;
                        superReservacion.TipoPago = "Card";
                        break;
                    //Check
                    case 'C':
                        superReservacion.Discount = await GetDiscount(5);
                        superReservacion.Pago.Numero_Pago = superReservacion.Cheque.Id;
                        superReservacion.TipoPago = "Check";
                        break;
                }//Fin del switch


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

                //Se llama al método para extraer tipo de cambio
                await extraerTipoCambio();
                superReservacion.MontoColones = Decimal.Multiply(superReservacion.Reservacion.Total, tipoCambio.venta);

                //Monto del adelanto
                superReservacion.Adelanto = Decimal.Multiply(superReservacion.Reservacion.Total, porcentajeAdelanto);



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
                superReservacion.Cheque = null;
                return true;
            }

        }

        private async Task<bool> RegistrarPago()
        {
            if (await RegistrarReservacion() > 0)
            {
                superReservacion.Pago.Id_Cliente = superReservacion.Cliente.Cedula;
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
                superReservacion.Reservacion.Id_Cliente = superReservacion.Cliente.Cedula;
                superReservacion.Reservacion.Id_Paquete = superReservacion.Paquete.Id;
                superReservacion.Reservacion.Id_Descuento = superReservacion.Discount.Id;
                superReservacion.Reservacion.Fecha_Registro = DateTime.Now;

                HttpResponseMessage response = await httpClient.PostAsJsonAsync<Reservacion>("api/Reservaciones/Agregar", superReservacion.Reservacion);

                //Reservacion reservacion = new Reservacion();
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
        private async Task<bool> extraerTipoCambio()
        {
            try
            {
                //Se consume el método para la API
                HttpResponseMessage response = await clientGometa.GetAsync("tdc/tdc.json");

                //Se valida si todo fue correcto
                if (response.IsSuccessStatusCode)
                {
                    //Se realiza lectura de los datos en formato JSON
                    var result = response.Content.ReadAsStringAsync().Result;

                    //Se convierte el JSON en un Objeto TipoCambio
                    tipoCambio = JsonConvert.DeserializeObject<TipoCambio>(result);
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return false;
        }//Extraertipocambio

        /// <summary>
        /// Método para extraer los datos del api de cedulas
        /// </summary>
        [HttpGet]
        private async Task<bool> extraerDatosCedula(string cedula)
        {
            try
            {
                //Se consume el método para la API
                HttpResponseMessage response = await clientGometa.GetAsync($"cedulas/{cedula}");

                //Se valida si todo fue correcto
                if (response.IsSuccessStatusCode)
                {
                    //Se realiza lectura de los datos en formato JSON
                    var result = response.Content.ReadAsStringAsync().Result;

                    // Convertir el JSON en un objeto anónimo para acceder a 'results'
                    var jsonObject = JsonConvert.DeserializeAnonymousType(result, new { results = new ClienteGometa[] { } });


                    if (jsonObject.results != null)
                    {
                        //Se convierte el JSON en un Objeto TipoCambio
                        clienteGometa = jsonObject.results[0];
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeCedula"] = "Error al registrar la cédula";
                return false;
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

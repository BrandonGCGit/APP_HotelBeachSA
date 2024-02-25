using APP_HotelBeachSA.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace APP_HotelBeachSA.Controllers
{
    public class AdminReservacionesController : Controller
    {

        private HotelBeachAPI hotelBeachAPI;

        private HttpClient client;

        public AdminReservacionesController()
        {
            hotelBeachAPI = new HotelBeachAPI();

            client = hotelBeachAPI.Inicial();
        }

        // GET: /api/Reservaciones/Listado
        public async Task<IActionResult> Index()
        {

            List<Reservacion> listado = new List<Reservacion>();

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await client.GetAsync("/api/Reservaciones/Listado");

            if (response.IsSuccessStatusCode)
            {
                var resultados = response.Content.ReadAsStringAsync().Result;

                listado = JsonConvert.DeserializeObject<List<Reservacion>>(resultados);
            }
            return View(listado);
        }

        // GET: /api/Reservaciones/Consultar?Id=1
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservacion = new Reservacion();

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage respuesta = await client.GetAsync($"/api/Reservaciones/Consultar?Id={id}");

            if (respuesta.IsSuccessStatusCode)
            {
                var resultado = respuesta.Content.ReadAsStringAsync().Result;

                reservacion = JsonConvert.DeserializeObject<Reservacion>(resultado);
            }
            return View(reservacion);
        }

        //GET: /api/Reservaciones/Agregar
        public IActionResult Create()
        {
            return View();
        }

        // POST: /api/Reservaciones/Agregar
        [HttpPost]
        [ValidateAntiForgeryToken]
        //TODO: Tenemos que obtener el id del usuario con el SESSION
        public async Task<IActionResult> Create([Bind] Reservacion reservacion)
        {
            reservacion.Id = 0;
            reservacion.Fecha_Registro = DateTime.Now;

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            var agregar = client.PostAsJsonAsync<Reservacion>("/api/Reservaciones/Agregar", reservacion);

            await agregar;

            var resultado = agregar.Result;

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["MensajeDiscount"] = "No se logro registrar el paquete...";
                return View(reservacion);
            }
        }

        // GET: Pago/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            var reservacion = new Reservacion();

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await client.GetAsync($"/api/Reservaciones/Consultar?Id={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;
                reservacion = JsonConvert.DeserializeObject<Reservacion>(resultado);
            }
            return View(reservacion);
        }

        // POST/api/Reservaciones/Modificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind] Reservacion reservacion)
        {
            if (id != reservacion.Id)
            {
                return NotFound();
            }

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            var modificar = client.PutAsJsonAsync<Reservacion>($"/api/Reservaciones/Modificar", reservacion);

            await modificar;

            var resultado = modificar.Result;

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Mensaje"] = "Datos Incorrectos";
                return View(reservacion);
            }
        }

        // GET: /api/Reservaciones/Eliminar?Id=2
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservacion = new Reservacion();

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await client.GetAsync($"/api/Reservaciones/Consultar?Id={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                reservacion = JsonConvert.DeserializeObject<Reservacion>(resultado);
            }
            return View(reservacion);
        }

        // POST: /api/Pagos/Eliminar?Id=2
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await client.DeleteAsync($"/api/Reservaciones/Eliminar?Id={id}");
            return RedirectToAction(nameof(Index));
        }

        private AuthenticationHeaderValue AutorizacionToken()
        {
            var token = HttpContext.Session.GetString("token");

            AuthenticationHeaderValue autorizacion = null;
            if (token != null && token.Length != 0)
            {
                autorizacion = new AuthenticationHeaderValue("Bearer", token);
            }
            return autorizacion;
        }
    }
}

using APP_HotelBeachSA.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace APP_HotelBeachSA.Controllers
{
    public class ChequesController : Controller
    {

        private HotelBeachAPI hotelBeachAPI;

        private HttpClient client;

        public ChequesController()
        {
            hotelBeachAPI = new HotelBeachAPI();

            client = hotelBeachAPI.Inicial();
        }

        // GET: /api/Cheques/Consultar?Id=1
        public async Task<IActionResult> Index()
        {

            List<Cheque> listado = new List<Cheque>();

            HttpResponseMessage response = await client.GetAsync("/api/Cheques/Listado");

            if (response.IsSuccessStatusCode)
            {
                var resultados = response.Content.ReadAsStringAsync().Result;

                listado = JsonConvert.DeserializeObject<List<Cheque>>(resultados);
            }
            return View(listado);
        }

        // GET: api/Pagos/Constultar/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cheque = new Cheque();

            HttpResponseMessage respuesta = await client.GetAsync($"/api/Cheques/Consultar?Id={id}");

            if (respuesta.IsSuccessStatusCode)
            {
                var resultado = respuesta.Content.ReadAsStringAsync().Result;

                cheque = JsonConvert.DeserializeObject<Cheque>(resultado);
            }
            return View(cheque);
        }

        //GET: Paquete/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Discounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //TODO: Tenemos que obtener el id del usuario con el SESSION
        public async Task<IActionResult> Create([Bind] Cheque cheque)
        {
            cheque.Id = 0;


            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            var agregar = client.PostAsJsonAsync<Cheque>("/api/Cheques/Agregar", cheque);

            await agregar;

            var resultado = agregar.Result;

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["MensajeDiscount"] = "No se logro registrar el paquete...";
                return View(cheque);
            }
        }

        // GET: Pago/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            var cheque = new Cheque();

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await client.GetAsync($"/api/Cheques/Consultar?Id={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;
                cheque = JsonConvert.DeserializeObject<Cheque>(resultado);
            }
            return View(cheque);
        }

        // POST/api/Cheques/Modificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind] Cheque cheque)
        {
            if (id != cheque.Id)
            {
                return NotFound();
            }

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            var modificar = client.PutAsJsonAsync<Cheque>($"/api/Cheques/Modificar", cheque);

            await modificar;

            var resultado = modificar.Result;

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Mensaje"] = "Datos Incorrectos";
                return View(cheque);
            }
        }

        // GET: Discounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cheque = new Cheque();

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await client.GetAsync($"/api/Cheques/Consultar?Id={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                cheque = JsonConvert.DeserializeObject<Cheque>(resultado);
            }
            return View(cheque);
        }

        // POST: /api/Pagos/Eliminar?Id=2
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await client.DeleteAsync($"/api/Cheques/Eliminar?Id={id}");
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

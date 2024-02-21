using APP_HotelBeachSA.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APP_HotelBeachSA.Controllers
{
    public class PagosController : Controller
    {

        private HotelBeachAPI hotelBeachAPI;

        private HttpClient client;

        public PagosController()
        {
            hotelBeachAPI = new HotelBeachAPI();

            client = hotelBeachAPI.Inicial();
        }

        // GET: Pagos
        public async Task<IActionResult> Index()
        {

            List<Pago> listado = new List<Pago>();

            HttpResponseMessage response = await client.GetAsync("/api/Pagos/Listado");

            if (response.IsSuccessStatusCode)
            {
                var resultados = response.Content.ReadAsStringAsync().Result;

                listado = JsonConvert.DeserializeObject<List<Pago>>(resultados);
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

            var pago = new Pago();

            HttpResponseMessage respuesta = await client.GetAsync($"/api/Pagos/Consultar?Id={id}");

            if (respuesta.IsSuccessStatusCode)
            {
                var resultado = respuesta.Content.ReadAsStringAsync().Result;

                pago = JsonConvert.DeserializeObject<Pago>(resultado);
            }
            return View(pago);
        }

        //GET: Paquete/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Discounts/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //TODO: Tenemos que obtener el id del usuario con el SESSION
        // public async Task<IActionResult> Create([Bind] Pago pago)
        // {
        //     paquete.Id = 0;
        //     paquete.Id_Usuario = "987654321";
        //     paquete.Fecha_Registro = DateTime.Now;

        //     var agregar = client.PostAsJsonAsync<Paquete>("/api/Paquetes/Crear", paquete);

        //     await agregar;

        //     var resultado = agregar.Result;

        //     if (resultado.IsSuccessStatusCode)
        //     {
        //         return RedirectToAction("Index");
        //     }
        //     else
        //     {
        //         TempData["MensajeDiscount"] = "No se logro registrar el paquete...";
        //         return View(paquete);
        //     }
        // }

        // GET: Pago/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            var pago = new Pago();

            HttpResponseMessage response = await client.GetAsync($"/api/Pagos/Consultar?Id={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                pago = JsonConvert.DeserializeObject<Pago>(resultado);
            }
            return View(pago);
        }

        // POST:/api/Pagos/Modificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind] Pago pago)
        {
            if (id != pago.Id)
            {
                return NotFound();
            }

            var modificar = client.PutAsJsonAsync<Pago>($"/api/Pagos/Modificar", pago);

            await modificar;

            var resultado = modificar.Result;

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Mensaje"] = "Datos Incorrectos";
                return View(pago);
            }
        }

        // GET: Discounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = new Pago();

            HttpResponseMessage response = await client.GetAsync($"/api/Pagos/Consultar?Id={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                pago = JsonConvert.DeserializeObject<Pago>(resultado);
            }
            return View(pago);
        }

        // POST: /api/Pagos/Eliminar?Id=2
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync($"/api/Pagos/Eliminar?Id={id}");
            return RedirectToAction(nameof(Index));
        }
    }
}

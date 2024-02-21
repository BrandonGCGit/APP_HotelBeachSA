using APP_HotelBeachSA.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APP_HotelBeachSA.Controllers
{
    public class PaquetesController : Controller
    {

        private HotelBeachAPI hotelBeachAPI;

        private HttpClient client;

        public PaquetesController()
        {
            hotelBeachAPI = new HotelBeachAPI();

            client = hotelBeachAPI.Inicial();
        }

        // GET: Paquetes
        public async Task<IActionResult> Index()
        {

            List<Paquete> listado = new List<Paquete>();

            HttpResponseMessage response = await client.GetAsync("/api/Paquetes/Listado");

            if (response.IsSuccessStatusCode)
            {
                var resultados = response.Content.ReadAsStringAsync().Result;

                listado = JsonConvert.DeserializeObject<List<Paquete>>(resultados);
            }
            return View(listado);
        }

        // GET: Paquetes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paquete = new Paquete();

            HttpResponseMessage respuesta = await client.GetAsync($"/api/Paquetes/Constultar?id={id}");

            if (respuesta.IsSuccessStatusCode)
            {
                var resultado = respuesta.Content.ReadAsStringAsync().Result;

                paquete = JsonConvert.DeserializeObject<Paquete>(resultado);
            }
            return View(paquete);
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
        public async Task<IActionResult> Create([Bind] Paquete paquete)
        {
            paquete.Id = 0;
            paquete.Id_Usuario = "987654321";
            paquete.Fecha_Registro = DateTime.Now;

            var agregar = client.PostAsJsonAsync<Paquete>("/api/Paquetes/Crear", paquete);

            await agregar;

            var resultado = agregar.Result;

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["MensajeDiscount"] = "No se logro registrar el paquete...";
                return View(paquete);
            }
        }

        // GET: Paquete/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            var paquete = new Paquete();

            HttpResponseMessage response = await client.GetAsync($"/api/Paquetes/Constultar?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                paquete = JsonConvert.DeserializeObject<Paquete>(resultado);
            }
            return View(paquete);
        }

        // POST: Discounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind] Paquete paquete)
        {
            if (id != paquete.Id)
            {
                return NotFound();
            }

            var modificar = client.PutAsJsonAsync<Paquete>($"/api/Paquetes/Editar?id={id}", paquete);

            await modificar;

            var resultado = modificar.Result;

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Mensaje"] = "Datos Incorrectos";
                return View(paquete);
            }
        }

        // GET: Discounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paquete = new Paquete();

            HttpResponseMessage response = await client.GetAsync($"/api/Paquetes/Constultar?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                paquete = JsonConvert.DeserializeObject<Paquete>(resultado);
            }
            return View(paquete);
        }

        // POST: Discounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync($"/api/Paquetes/Eliminar?id={id}");
            return RedirectToAction(nameof(Index));
        }
    }
}

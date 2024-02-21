using APP_HotelBeachSA.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APP_HotelBeachSA.Controllers
{
    public class RolesController : Controller
    {

        private HotelBeachAPI hotelBeachAPI;

        private HttpClient client;

        public RolesController()
        {
            hotelBeachAPI = new HotelBeachAPI();

            client = hotelBeachAPI.Inicial();
        }

        public async Task<IActionResult> Index()
        {
            List<Rol> listado = new List<Rol>();

            HttpResponseMessage response = await client.GetAsync("/api/Roles/Listado");

            if (response.IsSuccessStatusCode)
            {
                var resultados = response.Content.ReadAsStringAsync().Result;

                listado = JsonConvert.DeserializeObject<List<Rol>>(resultados);
            }

            return View(listado);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] Rol rol)
        {

            rol.Fecha_Registro = DateTime.Now;

            var agregar = client.PostAsJsonAsync<Rol>("/api/Roles/Agregar", rol);
            await agregar;

            var resultado = agregar.Result;

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                
                TempData["Mensaje"] = "No se logró registrar el rol.";

                //se ubica al usuairo dnetro de la view crear con los datos del libro
                return View(rol);
            }
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rol = new Rol();

            HttpResponseMessage respuesta = await client.GetAsync($"/api/Roles/Consultar?id={id}");

            if (respuesta.IsSuccessStatusCode)
            {
                var resultado = respuesta.Content.ReadAsStringAsync().Result;

                rol = JsonConvert.DeserializeObject<Rol>(resultado);
            }
            return View(rol);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rol = new Rol();

            HttpResponseMessage response = await client.GetAsync($"/api/Roles/Consultar?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                rol = JsonConvert.DeserializeObject<Rol>(resultado);
            }
            return View(rol);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync($"/api/Roles/Eliminar?id={id}");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {

            var rol = new Rol();

            HttpResponseMessage response = await client.GetAsync($"/api/Roles/Consultar?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                rol = JsonConvert.DeserializeObject<Rol>(resultado);
            }
            return View(rol);
        }

        // POST: Discounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind] Rol rol)
        {
            if (id != rol.Id)
            {
                return NotFound();
            }
            rol.Fecha_Registro = DateTime.Now;

            var modificar = client.PutAsJsonAsync<Rol>($"/api/Roles/Modificar?id={id}", rol);

            await modificar;

            var resultado = modificar.Result;

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Mensaje"] = "Datos Incorrectos";
                return View(rol);
            }
        }

    }
}

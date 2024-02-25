using APP_HotelBeachSA.Model;
using APP_HotelBeachSA.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace APP_HotelBeachSA.Controllers
{
    public class DiscountsController : Controller
    {

        private HotelBeachAPI hotelBeachAPI;

        private HttpClient client;

        private ServicesHotelBeachAPI servicesAPI;

        public DiscountsController()
        {
            hotelBeachAPI = new HotelBeachAPI();

            client = hotelBeachAPI.Inicial();

            servicesAPI = new ServicesHotelBeachAPI();
        }

        // GET: Discounts
        public async Task<IActionResult> Index()
        {

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            List<Discount> listado = new List<Discount>();

            HttpResponseMessage response = await client.GetAsync("Descuentos/Listado");

            if (response.IsSuccessStatusCode)
            {
                var resultados = response.Content.ReadAsStringAsync().Result;

                listado = JsonConvert.DeserializeObject<List<Discount>>(resultados);
            }
            return View(listado);
        }

        // GET: Discounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }


            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var discount = new Discount();

            HttpResponseMessage respuesta = await client.GetAsync($"Descuentos/Consultar?id={id}");

            if (respuesta.IsSuccessStatusCode)
            {
                var resultado = respuesta.Content.ReadAsStringAsync().Result;

                discount = JsonConvert.DeserializeObject<Discount>(resultado);
            }
            return View(discount);
        }

        //GET: Discounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Discounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //TODO: Tenemos que obtener el id del usuario con el SESSION
        public async Task<IActionResult> Create([Bind] Discount discount)
        {

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            discount.Id = 0;
            discount.Fecha_Registro = DateTime.Now;


            //var usuario = new Usuario();
            var email = HttpContext.Session.GetString("email");

            var usuario = servicesAPI.getUsuarioPorEmail(email);


            discount.Id_Usuario = usuario.Result.Cedula;


            var agregar = client.PostAsJsonAsync<Discount>("Descuentos/Agregar", discount);

            await agregar;

            var resultado = agregar.Result;

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["MensajeDiscount"] = "No se logro registrar el descuento...";
                return View(discount);
            }
        }

        // GET: Discounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {


            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var discount = new Discount();

            HttpResponseMessage response = await client.GetAsync($"Descuentos/Consultar?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                discount = JsonConvert.DeserializeObject<Discount>(resultado);
            }
            return View(discount);
        }

        // POST: Discounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Id_Usuario,Descuento,Noches,Fecha_Registro")] Discount discount)
        {
            if (id != discount.Id)
            {
                return NotFound();
            }

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var cedula_Usuario = HttpContext.Session.GetString("cedula");

            discount.Id_Usuario = cedula_Usuario;

            var modificar = client.PutAsJsonAsync<Discount>("Descuentos/Modificar", discount);

            await modificar;

            var resultado = modificar.Result;

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Mensaje"] = "Datos Incorrectos";
                return View(discount);
            }
        }

        // GET: Discounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var discount = new Discount();

            HttpResponseMessage response = await client.GetAsync($"Descuentos/Consultar?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                discount = JsonConvert.DeserializeObject<Discount>(resultado);
            }
            return View(discount);
        }

        // POST: Discounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await client.DeleteAsync($"Descuentos/Eliminar?id={id}");
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

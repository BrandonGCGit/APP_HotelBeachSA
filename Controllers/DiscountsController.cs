using APP_HotelBeachSA.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APP_HotelBeachSA.Controllers
{
    public class DiscountsController : Controller
    {

        private HotelBeachAPI hotelBeachAPI;

        private HttpClient client;

        public DiscountsController()
        {
            hotelBeachAPI = new HotelBeachAPI();

            client = hotelBeachAPI.Inicial();
        }

        // GET: Discounts
        public async Task<IActionResult> Index()
        {

            List<Discount> listado = new List<Discount>();

            HttpResponseMessage response = await client.GetAsync("/Descuentos/Listado");

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

            var discount = new Discount();

            HttpResponseMessage respuesta = await client.GetAsync($"/Descuentos/Consultar?id={id}");

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
        public async Task<IActionResult> create([Bind] Discount discount)
        {
            discount.Id = -1;
            discount.Id_Usuario = "987654321";

            var agregar = client.PostAsJsonAsync<Discount>("/Descuentos/Agregar", discount);
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

            var discount = new Discount();

            HttpResponseMessage response = await client.GetAsync($"/Descuentos/Consultar?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                discount = JsonConvert.DeserializeObject<Discount>(resultado);
            }
            return View(discount);
        }

        //// POST: Discounts/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Id_Usuario,Descuento,Noches,Fecha_Registro")] Discount discount)
        //{
        //    if (id != discount.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(discount);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!DiscountExists(discount.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(discount);
        //}

        //// GET: Discounts/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var discount = await _context.Discount
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (discount == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(discount);
        //}

        //// POST: Discounts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var discount = await _context.Discount.FindAsync(id);
        //    if (discount != null)
        //    {
        //        _context.Discount.Remove(discount);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool DiscountExists(int id)
        //{
        //    return _context.Discount.Any(e => e.Id == id);
        //}
    }
}

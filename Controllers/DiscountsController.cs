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

        //// GET: Discounts/Details/5
        //public async Task<IActionResult> Details(int? id)
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

        //// GET: Discounts/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Discounts/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Id_Usuario,Descuento,Noches,Fecha_Registro")] Discount discount)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(discount);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(discount);
        //}

        //// GET: Discounts/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var discount = await _context.Discount.FindAsync(id);
        //    if (discount == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(discount);
        //}

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

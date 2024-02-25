using APP_HotelBeachSA.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace APP_HotelBeachSA.Controllers
{
    public class ClientesController : Controller
    {
        private HotelBeachAPI hotelBeachAPI;
        private HttpClient client;

        public ClientesController()
        {
            hotelBeachAPI = new HotelBeachAPI();
            client = hotelBeachAPI.Inicial();
        }


        //CRUD CLIENTES START

        // GET: /api/Clientes/Listado
        public async Task<IActionResult> Index()
        {

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            List<Cliente> listado = new List<Cliente>();

            HttpResponseMessage response = await client.GetAsync("/api/Clientes/Listado");

            if (response.IsSuccessStatusCode)
            {
                var resultados = response.Content.ReadAsStringAsync().Result;

                listado = JsonConvert.DeserializeObject<List<Cliente>>(resultados);
            }
            return View(listado);
        }

        // GET: /api/Usuarios/Consultar?Cedula=
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            var cliente = new Cliente();

            HttpResponseMessage respuesta = await client.GetAsync($"/api/Clientes/Consultar?cedula={id}");

            if (respuesta.IsSuccessStatusCode)
            {
                var resultado = respuesta.Content.ReadAsStringAsync().Result;

                cliente = JsonConvert.DeserializeObject<Cliente>(resultado);
            }
            return View(cliente);
        }

        //GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /api/Clientes/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        //TODO: Tenemos que obtener el id del usuario con el SESSION
        public async Task<IActionResult> Create([Bind] Cliente cliente)
        {
            cliente.Fecha_Registro = DateTime.Now;

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            var agregar = client.PostAsJsonAsync<Cliente>("/api/Clientes/Crear", cliente);

            await agregar;

            var resultado = agregar.Result;

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["MensajeDiscount"] = "No se logro registrar el cliente.";
                return View(cliente);
            }
        }

        // GET: Paquete/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {

            var cliente = new Cliente();

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await client.GetAsync($"/api/Clientes/Consultar?cedula={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                cliente = JsonConvert.DeserializeObject<Cliente>(resultado);
            }
            return View(cliente);
        }

        // POST: /api/Clientes/Editar?cedula
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind] Cliente cliente)
        {
            if (id != cliente.Cedula)
            {
                return NotFound();
            }

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            var modificar = client.PutAsJsonAsync<Cliente>($"/api/Clientes/Editar?cedula={id}", cliente);

            await modificar;

            var resultado = modificar.Result;

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Mensaje"] = "Datos Incorrectos";
                return View(cliente);
            }
        }

        // GET: /api/Usuarios/Eliminar?Cedula=
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = new Cliente();

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await client.GetAsync($"/api/Clientes/Consultar?cedula={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                cliente = JsonConvert.DeserializeObject<Cliente>(resultado);
            }
            return View(cliente);
        }

        // POST: /api/Clientes/Eliminar?cedula=
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            client.DefaultRequestHeaders.Authorization = AutorizacionToken();
            HttpResponseMessage response = await client.DeleteAsync($"/api/Clientes/Eliminar?cedula={id}");
            return RedirectToAction(nameof(Index));
        }


        //CRUD CLIENTES END


        [HttpGet]
        public IActionResult CheckId()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckId(string cedula)
        {

            if (!string.IsNullOrWhiteSpace(cedula))
            {
                // Aquí se llamaría al API para verificar si la cédula existe
                var clienteExiste = false;

                if (!clienteExiste)
                {
                    // Si la cédula no existe, redirige directamente al formulario de registro
                    return RedirectToAction("Register", new { cedula });
                }
                else
                {
                    // Si la cédula existe, redirige a la sección de reservación
                    return RedirectToAction("Reservation", new { cedula });
                }
            }
            else
            {
                // Si el ID es null o vacío, redirige al usuario de vuelta al formulario de CheckCedula con el msj de error
                TempData["Mensaje"] = "Error registering account.";
                return RedirectToAction("CheckId");
            }
        }

        [HttpGet]
        public IActionResult Register(string cedula, string name)
        {
            var model = new Cliente { Cedula = cedula, Nombre = "Nombre del cliente" };

            return View(model);
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

using APP_HotelBeachSA.Model;
using APP_HotelBeachSA.Models;
using APP_HotelBeachSA.Models.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace APP_HotelBeachSA.Controllers
{
    public class UsuariosController : Controller
    {

        private HotelBeachAPI hotelBeachAPI;

        private HttpClient client;

        private ServicesHotelBeachAPI servicesAPI;

        public UsuariosController()
        {
            hotelBeachAPI = new HotelBeachAPI();

            client = hotelBeachAPI.Inicial();

            servicesAPI = new ServicesHotelBeachAPI();
        }

        //CRUD USUARIOS START
        // GET: api/Usuarios/Listado
        public async Task<IActionResult> Index()
        {

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            List<Usuario> listado = new List<Usuario>();

            HttpResponseMessage response = await client.GetAsync("/api/Usuarios/Listado");

            if (response.IsSuccessStatusCode)
            {
                var resultados = response.Content.ReadAsStringAsync().Result;

                listado = JsonConvert.DeserializeObject<List<Usuario>>(resultados);
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

            var usuario = new Usuario();

            HttpResponseMessage respuesta = await client.GetAsync($"/api/Usuarios/Consultar?Cedula={id}");

            if (respuesta.IsSuccessStatusCode)
            {
                var resultado = respuesta.Content.ReadAsStringAsync().Result;

                usuario = JsonConvert.DeserializeObject<Usuario>(resultado);
            }
            return View(usuario);
        }

        //GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /api/Usuarios/Agregar
        [HttpPost]
        [ValidateAntiForgeryToken]
        //TODO: Tenemos que obtener el id del usuario con el SESSION
        public async Task<IActionResult> Create([Bind] Usuario usuario)
        {

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            usuario.FechaRegistro = DateTime.Now;

            var agregar = client.PostAsJsonAsync<Usuario>("/api/Usuarios/Agregar", usuario);

            await agregar;

            var resultado = agregar.Result;

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["MensajeDiscount"] = "No se logro registrar el paquete...";
                return View(usuario);
            }
        }

        // GET: Paquete/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {


            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var usuario = new Usuario();

            HttpResponseMessage response = await client.GetAsync($"/api/Usuarios/Consultar?Cedula={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                usuario = JsonConvert.DeserializeObject<Usuario>(resultado);
            }
            return View(usuario);
        }

        // POST: /api/Usuarios/Modificar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind] Usuario usuario)
        {
            if (id != usuario.Cedula)
            {
                return NotFound();
            }

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var modificar = client.PutAsJsonAsync<Usuario>($"/api/Usuarios/Modificar", usuario);

            await modificar;

            var resultado = modificar.Result;

            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Mensaje"] = "Datos Incorrectos";
                return View(usuario);
            }
        }

        // GET: /api/Usuarios/Eliminar?Cedula=
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            var usuario = new Usuario();

            HttpResponseMessage response = await client.GetAsync($"/api/Usuarios/Consultar?Cedula={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                usuario = JsonConvert.DeserializeObject<Usuario>(resultado);
            }
            return View(usuario);
        }

        // POST: /api/Usuarios/Eliminar?Cedula=
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            HttpResponseMessage response = await client.DeleteAsync($"/api/Usuarios/Eliminar?Cedula={id}");
            return RedirectToAction(nameof(Index));
        }
        //CRUD USUARIOS END

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind] Usuario usuario)
        {
            AutorizacionResponse autorizacion = null;
            if (usuario == null)
            {
                return View();
            }

            HttpResponseMessage response = await client.PostAsync($"/api/Usuarios/Autenticar?email={usuario.Email}&password={usuario.Password}", null);

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                autorizacion = JsonConvert.DeserializeObject<AutorizacionResponse>(resultado);
            }

            if (autorizacion != null)
            {

                HttpContext.Session.SetString("token", autorizacion.Token);
                HttpContext.Session.SetString("email", usuario.Email);

                var usuarioId_rol = servicesAPI.getUsuarioPorEmail(usuario.Email).Result.Id_Rol;


                var rolesPermitidos = servicesAPI.ConsultarRol(usuarioId_rol).Result.Funciones;

                var rolesArray = rolesPermitidos.Split(',').Select(x => x.Trim()).ToArray();




                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, usuario.Email));

                foreach (var rol in rolesArray)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, rol));
                }
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("IndexAdmin", "Home");
            }
            else
            {
                return View(usuario);
            }

        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();// Cerrar Sesion
            HttpContext.Session.SetString("token", "");//Se borra el token
            return RedirectToAction("Index", "Home");//Se ubica al usuario en la pagina de inicio
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

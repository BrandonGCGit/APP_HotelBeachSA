using APP_HotelBeachSA.Model;
using APP_HotelBeachSA.Models;
using APP_HotelBeachSA.Models.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace APP_HotelBeachSA.Controllers
{
    public class UsuariosController : Controller
    {

        private HotelBeachAPI hotelBeachAPI;

        private HttpClient httpClient;

        public UsuariosController()
        {
            hotelBeachAPI = new HotelBeachAPI();

            httpClient = hotelBeachAPI.Inicial();
        }

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

            HttpResponseMessage response = await httpClient.PostAsync($"/api/Usuarios/Autenticar?email={usuario.Email}&password={usuario.Password}", null);

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                autorizacion = JsonConvert.DeserializeObject<AutorizacionResponse>(resultado);
            }

            if (autorizacion != null)
            {
                HttpContext.Session.SetString("token", autorizacion.Token);


                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, usuario.Email));

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

    }
}

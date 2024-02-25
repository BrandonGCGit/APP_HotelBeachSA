using APP_HotelBeachSA.Model;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace APP_HotelBeachSA.Models
{
    public class ServicesHotelBeachAPI
    {


        private HotelBeachAPI hotelBeachAPI;

        private HttpClient httpClient;

        public ServicesHotelBeachAPI()
        {
            hotelBeachAPI = new HotelBeachAPI();

            httpClient = hotelBeachAPI.Inicial();
        }

        /// <summary>
        /// Metodo para encontrar un usuario por email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Retunra un Usuario</returns>
        public async Task<Usuario> getUsuarioPorEmail(string email)
        {

            var usuario = new Usuario();

            HttpResponseMessage respuesta = await httpClient.GetAsync($"/api/Usuarios/ConsultarPorEmail?email={email}");

            if (respuesta.IsSuccessStatusCode)
            {
                var usuarioJson = respuesta.Content.ReadAsStringAsync().Result;

                usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);

            }

            return usuario;

        }

        public async Task<Rol> ConsultarRol(int id)
        {

            var rol = new Rol();

            HttpResponseMessage respuesta = await httpClient.GetAsync($"/api/Roles/Consultar?id={id}");

            if (respuesta.IsSuccessStatusCode)
            {
                var RolJson = respuesta.Content.ReadAsStringAsync().Result;

                rol = JsonConvert.DeserializeObject<Rol>(RolJson);

            }

            return rol;

        }
    }
}

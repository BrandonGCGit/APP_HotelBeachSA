namespace APP_HotelBeachSA.Models
{
    public class GometaAPI
    {
        public HttpClient Inicial()
        {
            //Variable para manejar el objeto HttpClient
            var client = new HttpClient();

            //URL Nube
            client.BaseAddress = new Uri("https://apis.gometa.org");


            //Se retorna el object
            return client;
        }//inicial
    }
}

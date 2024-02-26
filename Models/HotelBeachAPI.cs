namespace APP_HotelBeachSA.Models
{
    public class HotelBeachAPI
    {

        public HttpClient Inicial()
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri("https://www.ApiBeachHotel.somee.com");
            //client.BaseAddress = new Uri("https://localhost:7216/");

            return client;
        }

    }
}

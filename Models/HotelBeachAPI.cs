namespace APP_HotelBeachSA.Models
{
    public class HotelBeachAPI
    {

        public HttpClient Inicial()
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://www.ApiBeachHotel.somee.com");

            return client;
        }

    }
}

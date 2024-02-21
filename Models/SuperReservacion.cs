namespace APP_HotelBeachSA.Models
{
    public class SuperReservacion
    {
        

        public Cliente Cliente { get; set; }
        public Reservacion Reservacion { get; set; }
        public Paquete Paquete { get; set; }
        public Pago Pago { get; set; }
        public Cheque Cheque { get; set; }
        public Discount Discount { get; set; }
       
    }
}

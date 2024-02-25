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

        public decimal Adelanto { get; set; }
        public decimal CostoPersona { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal MontoDescuento { get; set; }
        public decimal Iva { get; set; }
        public decimal MontoColones { get; set; }
        public string TipoPago { get; set; }

    }
}

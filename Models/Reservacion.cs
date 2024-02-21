using System.ComponentModel.DataAnnotations;

namespace APP_HotelBeachSA.Models
{
    public class Reservacion
    {
        [Key]
        public int Id { get; set; }
        public string Id_Cliente { get; set; }
        public int Id_Paquete { get; set; }
        public int Id_Descuento { get; set; }
        public int Huespedes { get; set; }
        public decimal Total { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Entrada { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Salida { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Fecha_Registro { get; set; }
    }
}

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

        [Required(ErrorMessage = "Select the number of guests")]
        public int Huespedes { get; set; }
        public int Noches { get; set; }
        public decimal Total { get; set; }

        [Required(ErrorMessage = "Select the Check-in")]
        [DataType(DataType.DateTime)]
        public DateTime Entrada { get; set; }

        [Required(ErrorMessage = "Select the Check-out")]
        [DataType(DataType.DateTime)]
        public DateTime Salida { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Fecha_Registro { get; set; }
    }
}

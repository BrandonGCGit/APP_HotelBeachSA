using System.ComponentModel.DataAnnotations;

namespace APP_HotelBeachSA.Models
{
    public class Pago
    {
        [Key]
        public int Id { get; set; }
        public string Id_Cliente { get; set; }
        public int Id_Reservacion { get; set; }

        [Required(ErrorMessage = "Select the Type of Payment")]
        public char Tipo_Pago { get; set; }
        public int Numero_Pago { get; set; }
        public DateTime Fecha_Registro { get; set; }

    }
}

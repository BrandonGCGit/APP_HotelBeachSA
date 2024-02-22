using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APP_HotelBeachSA.Models
{
    public class Cliente
    {
        [Key]
<<<<<<< Updated upstream
        public string Cedula { get; set; }
        public string Tipo_Cedula { get; set; }
        public string Nombre { get; set; }
        public string Primer_Apellido { get; set; }
        public string Segundo_Apellido { get; set; }
        public int Telefono { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }

        [DataType(DataType.DateTime)]
=======
        [Required(ErrorMessage = "Please enter your ID number.")]
        [DisplayName("Identification Number")]
        [RegularExpression(@"^(\d{9}|\d{10}|\d{11}|\d{12})$", ErrorMessage = "The identification number format is not valid. The accepted formats are: Physical ID (9 digits), Legal ID and NITE (10 digits), DIMEX ID (11 or 12 digits)")]
        public int Id { get; set; }

        [Required(ErrorMessage = "The phone number is required.")]
        [DisplayName("Phone Number")]
        [RegularExpression(@"^(\d{8})$", ErrorMessage = "Invalid phone format, the phone number must have 8 digits")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "The address is required.")]
        [StringLength(100, ErrorMessage = "The maximum length is 100 characters")]
        [DisplayName("Address")]
        [RegularExpression(@"^[a-zA-Z0-9\s,.\-/()]+$", ErrorMessage = "Invalid address format")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Id_Cliente { get; set; }
        public int Id_Paquete { get; set; }
        public int Id_Descuento { get; set; }
        public int Huespedes { get; set; }
        public decimal Total { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime Salida { get; set; }
>>>>>>> Stashed changes
        public DateTime Fecha_Registro { get; set; }
    }
}

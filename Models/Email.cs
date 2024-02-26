using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using APP_HotelBeachSA.Model;
using APP_HotelBeachSA.Models;
using iText.Html2pdf; // Asegúrate de tener iText7 y su extensión HTML2PDF

namespace APP_HotelBeachSA.Models
{
    public class Email
    {
        public void Enviar(SuperReservacion superReservacion)
        {
            try
            {

                var htmlContent = $@"
                
                <body>
                    <div class=""container"">
                        <h1>Reservation</h1>
                        <h4>Thank you for selecting us</h4>

                        <div>    
                           <h5>Information about the Reservation</h5>
                           <p>Check-in: {superReservacion.Reservacion.Entrada}</p>
                           <p>Check-out: {superReservacion.Reservacion.Salida}</p>
                           <p>Number of guests: {superReservacion.Reservacion.Huespedes}</p>
                           <p>Nights: {superReservacion.Reservacion.Noches}</p>
                           <p>Total: {superReservacion.Reservacion.Total}</p>
                        </div>

                        <div>    
                           <h5>Information about the Package</h5>
                           <p>Package: {superReservacion.Paquete.Nombre}</p>
                           <p>Cost per person: {superReservacion.Paquete.Costo_Persona}</p>
                           <p>Advance %: {superReservacion.Paquete.Adelanto}</p>
                           <p>Terms: {superReservacion.Paquete.Terminos_Pago}</p>
                        </div>

                         <div>    
                           <h5>Information about the Payment</h5>
                           <p>Type of Payment: {superReservacion.TipoPago}</p>
                           <p>Number of payment: {superReservacion.Pago.Numero_Pago}</p>
                           <p>Cost per person: {superReservacion.CostoPersona}</p>
                           <p>Total Cost: {superReservacion.CostoTotal}</p>
                           <p>Iva: {superReservacion.Iva}</p>
                           <p>Discount: {superReservacion.MontoDescuento}</p>
                           <p>Total Price: {superReservacion.Reservacion.Total}</p>
                           <p>Colones: {superReservacion.MontoColones}</p>
                           <p>Advance: {superReservacion.Adelanto}</p>
                        </div>

                        <div>    
                           <h5>We are waiting for you</h5>
                           <p>Feel free to contact us</p>
                           <p>Email: beach.hotel.App@outlook.com</p>
                        </div>        

                    </div>
                </body>
                </html>";



                // Generar PDF
                var pdfPath = GenerarPDF(htmlContent);

                // Configuración del correo
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("beach.hotel.App@outlook.com"),
                    Subject = "Datos de registro en Hotel Beach",
                    Body = "A continuacion se adjunta un documento con la informacion de su reserva. En caso de presntar algun error, por favor comunicarse nuestra operadora. Muchas gracias",
                    IsBodyHtml = true
                };



                mail.To.Add(superReservacion.Cliente.Email);
                mail.To.Add("beach.hotel.App@outlook.com");

                // Adjuntar PDF
                if (!string.IsNullOrEmpty(pdfPath))
                {
                    Attachment attachment = new Attachment(pdfPath, MediaTypeNames.Application.Pdf);
                    mail.Attachments.Add(attachment);
                }

                // Enviar correo
                using (SmtpClient smtp = new SmtpClient("smtp-mail.outlook.com", 587))
                {
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("beach.hotel.App@outlook.com", "abdy.Beach.2024");
                    smtp.Send(mail);
                }

                //// Limpiar
                //if (File.Exists(pdfPath))
                //{
                //    File.Delete(pdfPath); // Eliminar el PDF después de enviarlo, si así lo deseas
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GenerarPDF(string htmlContent)
        {
            var pdfPath = Path.Combine(Path.GetTempPath(), $"Registro_{Guid.NewGuid()}.pdf");

            using (FileStream stream = new FileStream(pdfPath, FileMode.Create))
            {
                ConverterProperties props = new ConverterProperties();
                HtmlConverter.ConvertToPdf(htmlContent, stream, props);
            }

            return pdfPath;
        }
    }
}
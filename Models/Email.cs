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
        public void Enviar(Reservacion reserva, Cliente cliente)
        {
            try
            {
                //var htmlContent = $@"

                //    <p>Fecha Entrada: {reserva.Entrada}</p>
                //    <p>Fecha Entrada: {reserva.Salida}</p>
                //    <p>Fecha Entrada: {reserva.Total}</p>
                //    ";

                var htmlContent = $@"
                    <p>Prueba</p>";


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



                mail.To.Add(cliente.Email);
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
                    smtp.Credentials = new NetworkCredential("beach.hotel.App@outlook.com", "abdy.Beach2024");
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
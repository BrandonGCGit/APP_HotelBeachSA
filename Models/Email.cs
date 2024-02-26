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
                var htmlContent = new StringBuilder();

                htmlContent.Append($@"
                
                <html>
                <head>
                    <style>
                        table {{
                            border-collapse: collapse;
                            width: 100%;
                        }}
                        th, td {{
                            border: 1px solid #dddddd;
                            text-align: left;
                            padding: 8px;
                        }}
                        th {{
                            background-color: #f2f2f2;
                        }}
                    </style>
                </head>
                <body>
                    <div class=""container"">
                        <h1>Reservation</h1>
                        <h4>Thank you for selecting us</h4>

                        <div>
                            <h5>Information about the Client</h5>
                            <table>
                                <tr>
                                    <th>Client</th>
                                    <td>{superReservacion.Cliente.Nombre} {superReservacion.Cliente.Primer_Apellido} {superReservacion.Cliente.Segundo_Apellido}</td>
                                </tr>
                                <tr>
                                    <th>Identification</th>
                                    <td>{superReservacion.Cliente.Cedula}</td>
                                </tr>
                                <tr>
                                    <th>Phone</th>
                                    <td>{superReservacion.Cliente.Telefono}</td>
                                </tr>
                                <tr>
                                    <th>Email</th>
                                    <td>{superReservacion.Cliente.Email}</td>
                                </tr>
                                <tr>
                                    <th>Direction</th>
                                    <td>{superReservacion.Cliente.Direccion}</td>
                                </tr>
                            </table>
                        </div>

                        <div>
                            <h5>Information about the Reservation</h5>
                            <table>
                                <tr>
                                    <th>Check-in</th>
                                    <td>{superReservacion.Reservacion.Entrada}</td>
                                </tr>
                                <tr>
                                    <th>Check-out</th>
                                    <td>{superReservacion.Reservacion.Salida}</td>
                                </tr>
                                <tr>
                                    <th>Number of guests</th>
                                    <td>{superReservacion.Reservacion.Huespedes}</td>
                                </tr>
                                <tr>
                                    <th>Nights</th>
                                    <td>{superReservacion.Reservacion.Noches}</td>
                                </tr>
                                <tr>
                                    <th>Total</th>
                                    <td>{superReservacion.Reservacion.Total}</td>
                                </tr>
                            </table>
                        </div>

                        <div>
                            <h5>Information about the Package</h5>
                            <table>
                                <tr>
                                    <th>Package</th>
                                    <td>{superReservacion.Paquete.Nombre}</td>
                                </tr>
                                <tr>
                                    <th>Cost per person</th>
                                    <td>{superReservacion.Paquete.Costo_Persona}</td>
                                </tr>
                                <tr>
                                    <th>Advance</th>
                                    <td>{superReservacion.Paquete.Adelanto}%</td>
                                </tr>
                                <tr>
                                    <th>Terms </th>
                                    <td>{superReservacion.Paquete.Terminos_Pago} months</td>
                                </tr>
                            </table>
                        </div>

                       <div>
                            <h5>Information about the Payment</h5>
                            <table>
                                <tr>
                                    <th>Type of Payment</th>
                                    <td>{superReservacion.TipoPago}</td>
                                </tr>
                                <tr>
                                    <th>Number of payment</th>
                                    <td>{superReservacion.Pago.Numero_Pago}</td>
                                </tr>
                                <tr>
                                    <th>Cost per person </th>
                                    <td>{superReservacion.CostoPersona.ToString("0.00")}</td>
                                </tr>
                                <tr>
                                    <th>Total Cost  </th>
                                    <td>{superReservacion.CostoTotal.ToString("0.00")}</td>
                                </tr>
                                <tr>
                                    <th>Iva  </th>
                                    <td>{superReservacion.Iva.ToString("0.00")}</td>
                                </tr>
                                <tr>
                                    <th>Discount   </th>
                                    <td>{superReservacion.MontoDescuento.ToString("0.00")}</td>
                                </tr>
                                <tr>
                                    <th>Total Price    </th>
                                    <td>{superReservacion.Reservacion.Total.ToString("0.00")}</td>
                                </tr>
                                <tr>
                                    <th>Colones   </th>
                                    <td>{superReservacion.MontoColones.ToString("0.00")}</td>
                                </tr>
                                <tr>
                                    <th>Advance    </th>
                                    <td>{superReservacion.Adelanto.ToString("0.00")}</td>
                                </tr>
                            </table>
                        </div>");

                        if (superReservacion.Cheque != null)
                        {
                            htmlContent.Append(@$"
                                    <div>
                                        <h5>Information about the Check</h5>
                                        <table>
                                            <tr>
                                                <th>Check Number</th>
                                                <td>{superReservacion.Pago.Numero_Pago}</td>
                                            </tr>
                                            <tr>
                                                <th>Bank Name</th>
                                                <td>{superReservacion.Cheque.Nombre_Banco}</td>
                                            </tr>
                                            <tr>
                                                <th>Quantity<th>
                                                <td>{superReservacion.Cheque.Cantidad}</td>
                                            </tr>
                                            <tr>
                                                <th>Currency<th>
                                                <td>{superReservacion.Cheque.Moneda}</td>
                                            </tr>
                                        </table>
                                    </div>
                            ");
                        }


                htmlContent.Append(@"
                         <div>    
                           <h5>We are waiting for you</h5>
                           <p>Feel free to contact us</p>
                           <p>Email: beach.hotel.App@outlook.com</p>
                         </div>        

                    </div>
                </body>
                </html>");





                // Convertir StringBuilder a string para su uso en otros lugares
                string htmlString = htmlContent.ToString();

                // Generar PDF
                var pdfPath = GenerarPDF(htmlString);

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
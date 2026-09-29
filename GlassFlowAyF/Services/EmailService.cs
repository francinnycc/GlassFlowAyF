using System.Net;
using System.Net.Mail;

namespace GlassFlowAyF.Services
{
    public class EmailService
        : IEmailService
    {
        private readonly IConfiguration
            _configuration;


        public EmailService(
            IConfiguration configuration)
        {
            _configuration =
                configuration;
        }


        public async Task EnviarCorreoAsync(
            string destinatario,
            string asunto,
            string contenidoHtml)
        {
            var servidor =
                _configuration[
                    "Email:SmtpServer"];

            var puertoTexto =
                _configuration[
                    "Email:SmtpPort"];

            var usuario =
                _configuration[
                    "Email:Username"];

            var password =
                _configuration[
                    "Email:Password"];

            var remitente =
                _configuration[
                    "Email:From"];


            if (string.IsNullOrWhiteSpace(
                    servidor) ||
                string.IsNullOrWhiteSpace(
                    puertoTexto) ||
                string.IsNullOrWhiteSpace(
                    usuario) ||
                string.IsNullOrWhiteSpace(
                    password) ||
                string.IsNullOrWhiteSpace(
                    remitente))
            {
                throw new InvalidOperationException(
                    "La configuración de correo electrónico está incompleta.");
            }


            if (!int.TryParse(
                    puertoTexto,
                    out var puerto))
            {
                throw new InvalidOperationException(
                    "El puerto SMTP configurado no es válido.");
            }


            using var mensaje =
                new MailMessage();


            mensaje.From =
                new MailAddress(
                    remitente,
                    "GlassFlow A&F");


            mensaje.To.Add(
                destinatario);


            mensaje.Subject =
                asunto;


            mensaje.Body =
                contenidoHtml;


            mensaje.IsBodyHtml =
                true;


            using var cliente =
                new SmtpClient(
                    servidor,
                    puerto);


            cliente.EnableSsl =
                true;


            cliente.Credentials =
                new NetworkCredential(
                    usuario,
                    password);


            await cliente
                .SendMailAsync(
                    mensaje);
        }
    }
}
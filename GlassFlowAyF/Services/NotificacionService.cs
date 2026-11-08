using System.Net;
using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Services;

public interface INotificacionService
{
    /// <summary>Agrega (sin guardar) las notificaciones internas para la decisión indicada.</summary>
    void AgregarInternas(string decision, SolicitudCotizacion solicitud, Cotizacion cotizacion, string? comentario);

    /// <summary>Envía los correos de la decisión. Nunca lanza excepción: los fallos se registran.</summary>
    Task EnviarCorreosAsync(string decision, SolicitudCotizacion solicitud, Cotizacion cotizacion, string? comentario, string? numeroOrden);
}

public class NotificacionService : INotificacionService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _email;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NotificacionService> _logger;

    public NotificacionService(ApplicationDbContext context, IEmailService email,
        IConfiguration configuration, ILogger<NotificacionService> logger)
    {
        _context = context;
        _email = email;
        _configuration = configuration;
        _logger = logger;
    }

    public void AgregarInternas(string decision, SolicitudCotizacion solicitud, Cotizacion cotizacion, string? comentario)
    {
        var url = $"/RevisionCliente/Historial/{solicitud.Id}";
        var proyecto = $"solicitud #{solicitud.Id} ({solicitud.NombreCliente})";

        void Agregar(string rol, string tipo, string titulo, string mensaje) =>
            _context.Notificaciones.Add(new Notificacion
            {
                DestinatarioRol = rol,
                Tipo = tipo,
                Titulo = titulo,
                Mensaje = Recortar(mensaje, 1000),
                Url = url,
                SolicitudCotizacionId = solicitud.Id,
                Fecha = DateTime.Now
            });

        switch (decision)
        {
            case EstadosRevision.DecisionAprobada:
                var msg = $"El cliente aprobó la cotización #{cotizacion.Id} de la {proyecto} por {cotizacion.Total:N2}.";
                Agregar(EstadosRevision.RolAdministrador, "Aviso", "Propuesta aprobada por el cliente", msg);
                Agregar(EstadosRevision.RolCotizador, "Aviso", "Propuesta aprobada por el cliente", msg);
                Agregar(EstadosRevision.RolOperaciones, "Tarea", "Nueva orden preliminar para producción",
                    msg + " Planifique producción e instalación.");
                break;

            case EstadosRevision.DecisionModificacion:
                Agregar(EstadosRevision.RolCotizador, "Tarea", "Ajustar cotización solicitada por el cliente",
                    $"El cliente solicitó modificaciones a la cotización #{cotizacion.Id} de la {proyecto}: {comentario}");
                Agregar(EstadosRevision.RolAdministrador, "Aviso", "Solicitud de modificación",
                    $"Se reasignó al cotizador la {proyecto}.");
                break;

            case EstadosRevision.DecisionRechazada:
                var rech = $"El cliente rechazó la cotización #{cotizacion.Id} de la {proyecto}. Motivo: {comentario}";
                Agregar(EstadosRevision.RolAdministrador, "Aviso", "Propuesta rechazada por el cliente", rech);
                Agregar(EstadosRevision.RolCotizador, "Aviso", "Propuesta rechazada por el cliente", rech);
                break;
        }
    }

    public async Task EnviarCorreosAsync(string decision, SolicitudCotizacion solicitud, Cotizacion cotizacion,
        string? comentario, string? numeroOrden)
    {
        var internos = await DestinatariosInternosAsync(decision);
        var cliente = WebUtility.HtmlEncode(solicitud.NombreCliente);
        var texto = WebUtility.HtmlEncode(comentario ?? string.Empty);

        string asuntoInterno, cuerpoInterno, asuntoCliente, cuerpoCliente;
        switch (decision)
        {
            case EstadosRevision.DecisionAprobada:
                asuntoInterno = $"GlassFlow | Propuesta aprobada – Solicitud #{solicitud.Id}";
                cuerpoInterno = $"<p>{cliente} aprobó la cotización #{cotizacion.Id} por <strong>{cotizacion.Total:N2}</strong>.</p>" +
                                $"<p>Orden preliminar: <strong>{numeroOrden}</strong>. La versión de la cotización quedó congelada.</p>";
                asuntoCliente = "GlassFlow | Confirmación de aprobación de su propuesta";
                cuerpoCliente = $"<p>Hola {cliente},</p><p>Registramos la aprobación formal de su propuesta (cotización #{cotizacion.Id}).</p>" +
                                $"<p>Número de orden preliminar: <strong>{numeroOrden}</strong>.</p>" +
                                "<p><strong>Siguientes pasos:</strong></p><ol>" +
                                "<li>Confirme la compra y el método de pago desde la cotización aprobada.</li>" +
                                "<li>Nuestro equipo de operaciones planificará la fabricación.</li>" +
                                "<li>Le contactaremos para coordinar la fecha de instalación.</li></ol>";
                break;

            case EstadosRevision.DecisionModificacion:
                asuntoInterno = $"GlassFlow | Modificación solicitada – Solicitud #{solicitud.Id}";
                cuerpoInterno = $"<p>{cliente} solicitó cambios en la cotización #{cotizacion.Id}:</p><blockquote>{texto}</blockquote>";
                asuntoCliente = "GlassFlow | Recibimos su solicitud de modificación";
                cuerpoCliente = $"<p>Hola {cliente},</p><p>Recibimos sus observaciones. Un cotizador ajustará la propuesta y le notificaremos cuando la nueva versión esté lista.</p>";
                break;

            default:
                asuntoInterno = $"GlassFlow | Propuesta rechazada – Solicitud #{solicitud.Id}";
                cuerpoInterno = $"<p>{cliente} rechazó la cotización #{cotizacion.Id}.</p><p>Motivo:</p><blockquote>{texto}</blockquote>";
                asuntoCliente = "GlassFlow | Registro de rechazo de propuesta";
                cuerpoCliente = $"<p>Hola {cliente},</p><p>Registramos el rechazo de la propuesta. Gracias por sus comentarios; si desea una nueva alternativa puede contactarnos.</p>";
                break;
        }

        foreach (var destino in internos)
            await EnviarSeguroAsync(destino, asuntoInterno, cuerpoInterno);

        if (!string.IsNullOrWhiteSpace(solicitud.Correo))
            await EnviarSeguroAsync(solicitud.Correo, asuntoCliente, cuerpoCliente);
    }

    private async Task<List<string>> DestinatariosInternosAsync(string decision)
    {
        var admins = await (
            from ur in _context.UserRoles
            join r in _context.Roles on ur.RoleId equals r.Id
            join u in _context.Users on ur.UserId equals u.Id
            where r.Name == EstadosRevision.RolAdministrador && u.Activo && u.Email != null
            select u.Email!).ToListAsync();

        var destinos = new List<string>(admins);
        AgregarConfig(destinos, "Notificaciones:CotizadorEmail");
        if (decision == EstadosRevision.DecisionAprobada)
            AgregarConfig(destinos, "Notificaciones:OperacionesEmail");

        return destinos.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    private void AgregarConfig(List<string> destinos, string clave)
    {
        var valor = _configuration[clave];
        if (!string.IsNullOrWhiteSpace(valor)) destinos.Add(valor.Trim());
    }

    private async Task EnviarSeguroAsync(string destino, string asunto, string cuerpo)
    {
        try
        {
            await _email.EnviarCorreoAsync(destino, asunto, cuerpo);
        }
        catch (Exception ex)
        {
            // El correo es informativo: un fallo SMTP no revierte la decisión ya registrada.
            _logger.LogWarning(ex, "No se pudo enviar la notificación '{Asunto}' a {Destino}", asunto, destino);
        }
    }

    private static string Recortar(string texto, int max) => texto.Length <= max ? texto : texto[..max];
}

namespace GlassFlowAyF.Models;

/// <summary>
/// Estados utilizados por el Módulo 15 (Revisión y Aprobación del Cliente).
/// Centraliza los textos para evitar inconsistencias entre controladores y vistas.
/// </summary>
public static class EstadosRevision
{
    // ----- Solicitud / proyecto -----
    public const string Cotizado = "Cotizado";
    public const string AprobadoCliente = "Aprobado por el Cliente";
    public const string SolicitudModificacion = "Solicitud de Modificación";
    public const string RechazadaCliente = "Propuesta Rechazada por el Cliente";

    /// <summary>Etapas desde la aprobación del cliente (flujo de producción e instalación).</summary>
    public static readonly string[] FlujoProduccion =
        { AprobadoCliente, "En producción", "Instalación programada", "Instalado", "Finalizado" };

    // ----- Cotización -----
    public const string CotPendiente = "Pendiente";
    public const string CotAprobada = "Aprobada";
    public const string CotCambios = "Cambios solicitados";
    public const string CotRechazada = "Rechazada";

    // ----- Decisiones registradas en la bitácora -----
    public const string DecisionAprobada = "Aprobada";
    public const string DecisionModificacion = "Modificacion";
    public const string DecisionRechazada = "Rechazada";

    // ----- Roles destinatarios de notificaciones internas -----
    public const string RolAdministrador = "Administrador";
    public const string RolCotizador = "Cotizador";
    public const string RolOperaciones = "Operaciones";

    // ----- Mensajes exigidos por los criterios de aceptación -----
    public const string MensajeEnProceso =
        "La cotización y propuesta visual para su solicitud se encuentran en proceso de elaboración por nuestro equipo técnico. Le notificaremos cuando estén listas para su revisión";

    public const string MensajeConfirmarAprobacion =
        "¿Desea aprobar formalmente esta propuesta para autorizar el inicio del proceso?";

    public const string MensajeComentarioObligatorio =
        "Debe ingresar una justificación detallada o descripción de los cambios requeridos antes de enviar la solicitud";

    /// <summary>
    /// Estado del proyecto que corresponde a sus cotizaciones (null si no hay ninguna).
    /// Lo usan la sincronización de estados y el seguimiento de la solicitud.
    /// </summary>
    public static string? EstadoSegunCotizaciones(IEnumerable<Cotizacion>? cotizaciones)
    {
        var lista = cotizaciones?.ToList() ?? new List<Cotizacion>();
        if (lista.Count == 0) return null;
        if (lista.Any(c => c.Estado == CotAprobada)) return AprobadoCliente;

        var ultima = lista.OrderByDescending(c => c.FechaCreacion).ThenByDescending(c => c.Id).First();
        return ultima.Estado switch
        {
            CotCambios => SolicitudModificacion,
            CotRechazada => RechazadaCliente,
            _ => Cotizado
        };
    }

    public const int ComentarioMinimo = 10;
    public const int ComentarioMaximo = 2000;
}

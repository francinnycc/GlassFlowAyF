namespace GlassFlowAyF.Models.ViewModels;

/// <summary>Panel integrado de revisión: cotización + propuesta visual + bitácora.</summary>
public class RevisionPropuestaViewModel
{
    public SolicitudCotizacion Solicitud { get; set; } = null!;

    /// <summary>null → la propuesta aún no se ha generado (Empty State).</summary>
    public Cotizacion? Cotizacion { get; set; }

    public DisenoIA? Diseno { get; set; }

    public OrdenPreliminar? Orden { get; set; }

    public IReadOnlyList<RevisionCliente> Historial { get; set; } = Array.Empty<RevisionCliente>();

    public bool EsCliente { get; set; }

    public bool EsAdministrador { get; set; }

    public bool Vencida => Cotizacion != null && Cotizacion.FechaVencimiento.Date < DateTime.Today;

    public int DiasVigencia => Cotizacion == null ? 0 : (Cotizacion.FechaVencimiento.Date - DateTime.Today).Days;

    /// <summary>Solo el cliente dueño puede responder una cotización pendiente, activa, no congelada y vigente.</summary>
    public bool PuedeResponder => EsCliente && Cotizacion != null
        && Cotizacion.Estado == EstadosRevision.CotPendiente
        && Cotizacion.Activa && !Cotizacion.Congelada && !Vencida;

    /// <summary>Texto que explica por qué no se puede responder (si aplica).</summary>
    public string? MotivoBloqueo => Cotizacion == null || PuedeResponder || !EsCliente ? null
        : Cotizacion.Congelada ? "La propuesta fue aprobada y su versión quedó congelada."
        : !Cotizacion.Activa ? "La propuesta fue rechazada y se encuentra desactivada."
        : Cotizacion.Estado == EstadosRevision.CotCambios ? "Solicitó modificaciones. La aprobación se habilitará cuando el cotizador publique la nueva versión."
        : Vencida ? "La cotización está vencida. Contacte a nuestro equipo para renovarla."
        : "La propuesta no está disponible para respuesta.";
}

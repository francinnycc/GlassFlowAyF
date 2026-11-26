using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models;

/// <summary>
/// Bitácora inmutable de decisiones del cliente sobre una propuesta (solo inserciones).
/// Índice único (CotizacionId, VersionCotizacion): una versión de cotización recibe una sola decisión.
/// </summary>
public class RevisionCliente
{
    public int Id { get; set; }

    public int CotizacionId { get; set; }
    public Cotizacion? Cotizacion { get; set; }

    public int SolicitudCotizacionId { get; set; }
    public SolicitudCotizacion? SolicitudCotizacion { get; set; }

    public int? DisenoIAId { get; set; }
    public DisenoIA? DisenoIA { get; set; }

    [Required, StringLength(450)]
    public string UsuarioId { get; set; } = string.Empty;
    public ApplicationUser? Usuario { get; set; }

    [Required, StringLength(20)]
    public string Decision { get; set; } = string.Empty;

    [StringLength(EstadosRevision.ComentarioMaximo)]
    public string? Comentario { get; set; }

    [StringLength(40)] public string? EstadoSolicitudAnterior { get; set; }
    [StringLength(40)] public string? EstadoSolicitudNuevo { get; set; }
    [StringLength(40)] public string? EstadoCotizacionAnterior { get; set; }
    [StringLength(40)] public string? EstadoCotizacionNuevo { get; set; }

    public Guid VersionCotizacion { get; set; }

    public bool AceptoTerminos { get; set; }

    [StringLength(45)]
    public string? DireccionIp { get; set; }

    [StringLength(300)]
    public string? UserAgent { get; set; }

    public DateTime FechaUtc { get; set; } = DateTime.UtcNow;

    /// <summary>Copia JSON de la cotización y la propuesta en el momento de la decisión.</summary>
    public string? SnapshotJson { get; set; }
}

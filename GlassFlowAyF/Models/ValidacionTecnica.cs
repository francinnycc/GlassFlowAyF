using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models;

public class ValidacionTecnica
{
    public int Id { get; set; }
    public int SolicitudCotizacionId { get; set; }
    public SolicitudCotizacion? SolicitudCotizacion { get; set; }
    [Required, StringLength(256)]
    public string Autor { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Now;
    [Required, StringLength(40)]
    public string Resultado { get; set; } = string.Empty;
    [Required, StringLength(10)]
    public string Complejidad { get; set; } = string.Empty;
    [Required, StringLength(1500)]
    public string Observaciones { get; set; } = string.Empty;
    public bool RecomiendaVisita { get; set; }
    [StringLength(1500)]
    public string? InformacionSolicitada { get; set; }
    [StringLength(2000)]
    public string? RespuestaCliente { get; set; }
    public DateTime? FechaRespuesta { get; set; }
    [ConcurrencyCheck]
    public Guid Version { get; set; } = Guid.NewGuid();
}

public class RegistrarValidacion
{
    [Required, RegularExpression("^(Validada|Pendiente de información)$")]
    public string Resultado { get; set; } = "Validada";
    [Required, RegularExpression("^(Baja|Media|Alta)$")]
    public string Complejidad { get; set; } = "Baja";
    [Required, StringLength(1500)]
    public string Observaciones { get; set; } = string.Empty;
    public bool RecomiendaVisita { get; set; }
    [StringLength(1500)]
    public string? InformacionSolicitada { get; set; }
}

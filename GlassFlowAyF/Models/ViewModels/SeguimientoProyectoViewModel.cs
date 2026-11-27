namespace GlassFlowAyF.Models.ViewModels;

public record EtapaSeguimiento(string Etapa, string Referencia, string Estado, DateTime? Fecha, string? Alerta = null);
public class SeguimientoProyectoViewModel
{
    public SolicitudCotizacion Solicitud { get; set; } = null!;
    public List<EtapaSeguimiento> Etapas { get; set; } = new();
    public List<HistorialEstadoProyecto> Historial { get; set; } = new();
}

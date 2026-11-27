using GlassFlowAyF.Models;
namespace GlassFlowAyF.Models.ViewModels;

public class RealidadAumentadaViewModel
{
    public SolicitudCotizacion Solicitud { get; set; } = null!;
    public List<Material> Materiales { get; set; } = new();
    public List<ConfiguracionAR> Historial { get; set; } = new();
}

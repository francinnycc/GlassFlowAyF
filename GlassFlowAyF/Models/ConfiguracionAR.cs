using System.ComponentModel.DataAnnotations;
namespace GlassFlowAyF.Models;

public class ConfiguracionAR
{
    public int Id { get; set; }
    public int SolicitudCotizacionId { get; set; }
    public SolicitudCotizacion SolicitudCotizacion { get; set; } = null!;
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;
    [StringLength(20)] public string Forma { get; set; } = "baranda";
    [StringLength(20)] public string Acabado { get; set; } = "negro";
    [StringLength(20)] public string Vidrio { get; set; } = "claro";
    [StringLength(450)] public string UsuarioId { get; set; } = "";
    public DateTime FechaUtc { get; set; } = DateTime.UtcNow;
    public bool UsoAR { get; set; }
}

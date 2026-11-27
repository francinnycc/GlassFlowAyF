using System.ComponentModel.DataAnnotations;
namespace GlassFlowAyF.Models;

public class HistorialEstadoProyecto
{
    public int Id { get; set; }
    [Required, StringLength(60)] public string Entidad { get; set; } = "";
    public int RegistroId { get; set; }
    [StringLength(100)] public string Anterior { get; set; } = "";
    [StringLength(100)] public string Nuevo { get; set; } = "";
    [StringLength(450)] public string UsuarioId { get; set; } = "";
    public DateTime FechaUtc { get; set; } = DateTime.UtcNow;
}

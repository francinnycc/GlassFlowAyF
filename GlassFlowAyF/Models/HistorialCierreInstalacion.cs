using System.ComponentModel.DataAnnotations;
namespace GlassFlowAyF.Models
{
    public class HistorialCierreInstalacion
    {
        public int Id { get; set; }
        public int TrabajoInstalacionId { get; set; }
        public TrabajoInstalacion TrabajoInstalacion { get; set; } = null!;
        [Required, StringLength(60)] public string Accion { get; set; } = string.Empty;
        [StringLength(450)] public string? Comentario { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}

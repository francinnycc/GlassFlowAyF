using System.ComponentModel.DataAnnotations;
namespace GlassFlowAyF.Models
{
    public class EvidenciaInstalacion
    {
        public int Id { get; set; }
        public int TrabajoInstalacionId { get; set; }
        public TrabajoInstalacion TrabajoInstalacion { get; set; } = null!;
        [Required, StringLength(200)] public string NombreArchivo { get; set; } = string.Empty;
        [Required, StringLength(100)] public string TipoContenido { get; set; } = string.Empty;
        [StringLength(300)] public string? Descripcion { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}

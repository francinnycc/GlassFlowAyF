using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models
{
    public class SolicitudFotografia
    {
        public int Id { get; set; }

        [Required]
        public int SolicitudCotizacionId { get; set; }

        public SolicitudCotizacion? SolicitudCotizacion { get; set; }

        [Required]
        [StringLength(500)]
        public string RutaArchivo { get; set; } = string.Empty;

        [StringLength(255)]
        public string? NombreOriginal { get; set; }

        [StringLength(100)]
        public string? TipoContenido { get; set; }

        public DateTime FechaCarga { get; set; } = DateTime.Now;
    }
}
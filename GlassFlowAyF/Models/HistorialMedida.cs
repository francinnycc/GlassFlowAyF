using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlassFlowAyF.Models
{
    public class HistorialMedida
    {
        public int Id { get; set; }

        [Required]
        public int SolicitudCotizacionId { get; set; }

        public SolicitudCotizacion? SolicitudCotizacion { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Ancho { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Alto { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Profundidad { get; set; }

        public int Cantidad { get; set; }

        [StringLength(100)]
        public string? UsuarioRegistro { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}
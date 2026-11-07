using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models
{
    public class DecisionPropuesta
    {
        public int Id { get; set; }

        [Required]
        public int SolicitudCotizacionId { get; set; }

        public SolicitudCotizacion? SolicitudCotizacion { get; set; }

        [Required]
        public int CotizacionId { get; set; }

        public Cotizacion? Cotizacion { get; set; }

        public int? DisenoIAId { get; set; }

        public DisenoIA? DisenoIA { get; set; }

        [Required]
        [StringLength(255)]
        public string ClienteId { get; set; } = string.Empty;

        public ApplicationUser? Cliente { get; set; }

        [Required]
        [StringLength(40)]
        public string Decision { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Comentario { get; set; }

        public bool AceptaTerminos { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;

        [StringLength(64)]
        public string? DireccionIP { get; set; }

        public Guid VersionCotizacion { get; set; }

        [StringLength(40)]
        public string? EstadoAnterior { get; set; }

        [StringLength(40)]
        public string? EstadoNuevo { get; set; }
    }
}

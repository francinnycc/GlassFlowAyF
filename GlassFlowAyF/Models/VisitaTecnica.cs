using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models
{
    public class VisitaTecnica
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Solicitud")]
        public int SolicitudCotizacionId { get; set; }

        public SolicitudCotizacion? SolicitudCotizacion { get; set; }

        [Required]
        [Display(Name = "Fecha y hora")]
        public DateTime FechaHora { get; set; }

        [Required]
        [StringLength(120)]
        [Display(Name = "Técnico asignado")]
        public string TecnicoAsignado { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string Direccion { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Observaciones { get; set; }

        [StringLength(40)]
        public string Estado { get; set; } = "Programada";
    }
}
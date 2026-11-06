using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models
{
    /// <summary>Bitácora de auditoría: fecha, hora y usuario de cada cambio en la propuesta.</summary>
    public class PropuestaVisualBitacora
    {
        public int Id { get; set; }

        [Required]
        public int PropuestaVisualId { get; set; }
        public PropuestaVisual? PropuestaVisual { get; set; }

        [Required]
        [StringLength(60)]
        public string Accion { get; set; } = string.Empty;   // Creación, Actualización, Archivo adjuntado...

        [StringLength(500)]
        public string? Detalle { get; set; }

        [Required]
        [StringLength(256)]
        public string Usuario { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}

using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models
{
    /// <summary>Imagen, referencia, plano o simulación adjunta a una propuesta visual.</summary>
    public class PropuestaVisualArchivo
    {
        public int Id { get; set; }

        [Required]
        public int PropuestaVisualId { get; set; }
        public PropuestaVisual? PropuestaVisual { get; set; }

        [Required]
        [StringLength(500)]
        public string RutaArchivo { get; set; } = string.Empty;

        [StringLength(255)]
        public string? NombreOriginal { get; set; }

        [StringLength(100)]
        public string? TipoContenido { get; set; }

        [StringLength(10)]
        public string Extension { get; set; } = string.Empty;

        public long TamanoBytes { get; set; }

        [StringLength(256)]
        public string SubidoPor { get; set; } = string.Empty;

        public DateTime FechaCarga { get; set; } = DateTime.Now;
    }
}

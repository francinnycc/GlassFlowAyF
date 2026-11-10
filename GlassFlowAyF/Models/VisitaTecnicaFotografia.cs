using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models
{
    public class VisitaTecnicaFotografia
    {
        public int Id { get; set; }

        [Required]
        public int VisitaTecnicaId { get; set; }

        public VisitaTecnica? VisitaTecnica { get; set; }

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
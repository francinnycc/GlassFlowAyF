using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models
{
    public class DisenoIARevision
    {
        public int Id { get; set; }
        public int DisenoIAId { get; set; }
        public DisenoIA DisenoIA { get; set; } = null!;
        [Required, StringLength(30)]
        public string Estado { get; set; } = string.Empty;
        [StringLength(2000)]
        public string? Comentario { get; set; }
        [Required, StringLength(450)]
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}

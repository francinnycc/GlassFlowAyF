using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlassFlowAyF.Models
{
    public class SoporteSolicitud
    {
        public int Id { get; set; }


        // ============================================
        // USUARIO
        // ============================================

        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        [ForeignKey(nameof(UsuarioId))]
        public ApplicationUser? Usuario { get; set; }


        // ============================================
        // INFORMACIÓN DEL TICKET
        // ============================================

        [Required]
        [StringLength(20)]
        public string Tipo { get; set; } = "Consulta";


        [Required]
        [StringLength(80)]
        public string Categoria { get; set; } = string.Empty;


        [Required]
        [StringLength(150)]
        public string Asunto { get; set; } = string.Empty;


        [Required]
        [StringLength(2000)]
        public string Mensaje { get; set; } = string.Empty;


        // ============================================
        // SEGUIMIENTO
        // ============================================

        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = "Pendiente";


        public DateTime FechaCreacion { get; set; } = DateTime.Now;


        public DateTime? FechaRespuesta { get; set; }


        [StringLength(3000)]
        public string? Respuesta { get; set; }


        public string? RespondidoPorId { get; set; }


        // ============================================
        // CALIFICACIÓN
        // ============================================

        [Range(1, 5)]
        public int? Calificacion { get; set; }


        [StringLength(1000)]
        public string? ComentarioCalificacion { get; set; }


        public DateTime? FechaCalificacion { get; set; }
    }
}
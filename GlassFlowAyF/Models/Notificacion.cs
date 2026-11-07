using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models
{
    public class Notificacion
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string? UsuarioId { get; set; }

        public ApplicationUser? Usuario { get; set; }

        [StringLength(40)]
        public string? RolDestino { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Mensaje { get; set; } = string.Empty;

        [Required]
        [StringLength(40)]
        public string Tipo { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Referencia { get; set; }

        public bool Leida { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}

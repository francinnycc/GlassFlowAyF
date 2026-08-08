using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlassFlowAyF.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(80)]
        public string Categoria { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        [Display(Name = "Precio base")]
        public decimal PrecioBase { get; set; }

        public bool Activo { get; set; } = true;
    }
}
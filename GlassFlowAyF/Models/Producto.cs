using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlassFlowAyF.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(120)]
        [Display(Name = "Nombre del producto")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [StringLength(80)]
        public string Categoria { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        [Display(Name = "Precio base")]
        [Range(0, 999999999)]
        public decimal PrecioBase { get; set; }

        [StringLength(500)]
        [Display(Name = "Imagen")]
        public string? ImagenUrl { get; set; }

        [Display(Name = "Permite instalación")]
        public bool PermiteInstalacion { get; set; } = true;

        [Display(Name = "Producto activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public ICollection<ProductoMaterial> ProductoMateriales { get; set; }
            = new List<ProductoMaterial>();

        public ICollection<SolicitudCotizacion> Solicitudes { get; set; }
            = new List<SolicitudCotizacion>();
    }
}
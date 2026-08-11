using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlassFlowAyF.Models
{
    public class Material
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(120)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo es obligatorio.")]
        [StringLength(80)]
        public string Tipo { get; set; } = string.Empty;

        [StringLength(80)]
        public string? Color { get; set; }

        [StringLength(100)]
        [Display(Name = "Perfil / Marco")]
        public string? Perfil { get; set; }

        [StringLength(100)]
        public string? Acabado { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        [Display(Name = "Grosor (mm)")]
        [Range(0, 100)]
        public decimal? Grosor { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        [Display(Name = "Precio adicional")]
        [Range(0, 999999999)]
        public decimal PrecioAdicional { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Display(Name = "Disponible")]
        public bool Activo { get; set; } = true;

        public ICollection<ProductoMaterial> ProductoMateriales { get; set; }
            = new List<ProductoMaterial>();

        public ICollection<SolicitudCotizacion> Solicitudes { get; set; }
            = new List<SolicitudCotizacion>();
    }
}
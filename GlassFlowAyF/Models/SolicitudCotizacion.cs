using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlassFlowAyF.Models
{
    public class SolicitudCotizacion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        [Display(Name = "Nombre del cliente")]
        public string NombreCliente { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(30)]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        [StringLength(120)]
        [Display(Name = "Producto solicitado")]
        public string TipoProducto { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        [Display(Name = "Descripción del trabajo")]
        public string Descripcion { get; set; } = string.Empty;

        [Range(0.01, 10000)]
        [Display(Name = "Ancho aproximado")]
        public decimal? Ancho { get; set; }

        [Range(0.01, 10000)]
        [Display(Name = "Alto aproximado")]
        public decimal? Alto { get; set; }

        [Display(Name = "¿Requiere instalación?")]
        public bool RequiereInstalacion { get; set; }

        [StringLength(300)]
        public string? Direccion { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        [StringLength(40)]
        public string Estado { get; set; } = "Solicitado";

        [Display(Name = "Fecha de solicitud")]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(12,2)")]
        [Display(Name = "Cotización estimada")]
        public decimal? MontoEstimado { get; set; }
    }
}
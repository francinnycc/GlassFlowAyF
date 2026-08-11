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


        // PRODUCTO

        [Display(Name = "Producto")]
        public int? ProductoId { get; set; }

        public Producto? Producto { get; set; }

        // Se conserva para solicitudes antiguas.
        [StringLength(120)]
        [Display(Name = "Producto solicitado")]
        public string TipoProducto { get; set; } = string.Empty;


        // MATERIAL

        [Display(Name = "Material y acabado")]
        public int? MaterialId { get; set; }

        public Material? Material { get; set; }


        // DESCRIPCIÓN

        [Required(ErrorMessage = "Describa el trabajo que necesita.")]
        [StringLength(1000)]
        [Display(Name = "Descripción del trabajo")]
        public string Descripcion { get; set; } = string.Empty;


        // MEDIDAS

        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, 10000)]
        [Display(Name = "Ancho aproximado (m)")]
        public decimal? Ancho { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, 10000)]
        [Display(Name = "Alto aproximado (m)")]
        public decimal? Alto { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, 10000)]
        [Display(Name = "Profundidad aproximada (m)")]
        public decimal? Profundidad { get; set; }

        [Range(1, 100)]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; } = 1;


        // INSTALACIÓN

        [Display(Name = "¿Requiere instalación?")]
        public bool RequiereInstalacion { get; set; }

        [StringLength(300)]
        public string? Direccion { get; set; }


        // INFORMACIÓN ADICIONAL

        [StringLength(500)]
        public string? Observaciones { get; set; }

        [StringLength(40)]
        public string Estado { get; set; } = "Solicitado";

        [Display(Name = "Fecha de solicitud")]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(12,2)")]
        [Display(Name = "Cotización estimada")]
        public decimal? MontoEstimado { get; set; }


        // FOTOGRAFÍAS

        public ICollection<SolicitudFotografia> Fotografias { get; set; }
            = new List<SolicitudFotografia>();


        // COTIZACIONES

        public ICollection<Cotizacion> Cotizaciones { get; set; }
            = new List<Cotizacion>();
    }
}
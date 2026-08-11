using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlassFlowAyF.Models
{
    public class Cotizacion
    {
        public int Id { get; set; }

        [Required]
        public int SolicitudCotizacionId { get; set; }

        public SolicitudCotizacion? SolicitudCotizacion { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        [Display(Name = "Costo de materiales")]
        [Range(0, 999999999)]
        public decimal CostoMateriales { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        [Display(Name = "Mano de obra")]
        [Range(0, 999999999)]
        public decimal ManoObra { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        [Display(Name = "Costo de instalación")]
        [Range(0, 999999999)]
        public decimal CostoInstalacion { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        [Display(Name = "Otros costos")]
        [Range(0, 999999999)]
        public decimal OtrosCostos { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Impuesto (%)")]
        [Range(0, 100)]
        public decimal PorcentajeImpuesto { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Impuesto { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        [Display(Name = "Descuento")]
        [Range(0, 999999999)]
        public decimal Descuento { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Total { get; set; }

        [Required]
        [StringLength(40)]
        public string Estado { get; set; } = "Pendiente";

        [StringLength(1500)]
        [Display(Name = "Detalle técnico")]
        public string? DetalleTecnico { get; set; }

        [StringLength(1000)]
        [Display(Name = "Condiciones")]
        public string? Condiciones { get; set; }

        [StringLength(1000)]
        [Display(Name = "Comentario del cliente")]
        public string? ComentarioCliente { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Válida hasta")]
        public DateTime FechaVencimiento { get; set; }
            = DateTime.Now.AddDays(15);

        public DateTime? FechaRespuestaCliente { get; set; }

        public Compra? Compra { get; set; }
    }
}
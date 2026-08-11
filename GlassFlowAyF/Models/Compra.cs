using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlassFlowAyF.Models
{
    public class Compra
    {
        public int Id { get; set; }

        [Required]
        public int CotizacionId { get; set; }

        public Cotizacion? Cotizacion { get; set; }

        [Required]
        public string ClienteId { get; set; } = string.Empty;

        public ApplicationUser? Cliente { get; set; }

        [Required]
        [StringLength(40)]
        public string NumeroOrden { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Método de pago")]
        public string MetodoPago { get; set; } = string.Empty;

        [Required]
        [StringLength(40)]
        public string Estado { get; set; } = "Pendiente de pago";

        [Column(TypeName = "decimal(12,2)")]
        public decimal Total { get; set; }

        public DateTime FechaCompra { get; set; } = DateTime.Now;

        public DateTime? FechaPago { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public Factura? Factura { get; set; }

        public TrabajoInstalacion? TrabajoInstalacion { get; set; }
    }
}
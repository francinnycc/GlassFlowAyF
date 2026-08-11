using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlassFlowAyF.Models
{
    public class Factura
    {
        public int Id { get; set; }

        [Required]
        public int CompraId { get; set; }

        public Compra? Compra { get; set; }

        [Required]
        [StringLength(40)]
        public string NumeroFactura { get; set; } = string.Empty;

        public DateTime FechaEmision { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(12,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Impuesto { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Descuento { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Total { get; set; }

        [StringLength(40)]
        public string Estado { get; set; } = "Emitida";
    }
}
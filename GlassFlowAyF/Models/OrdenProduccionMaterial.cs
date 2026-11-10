using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GlassFlowAyF.Models;

public class OrdenProduccionMaterial
{
    public int Id { get; set; }
    public int OrdenProduccionId { get; set; }
    public OrdenProduccion OrdenProduccion { get; set; } = null!;
    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;
    [Column(TypeName = "decimal(12,2)")] public decimal Cantidad { get; set; }
    [Required, StringLength(30)] public string Unidad { get; set; } = "unidad";
    public bool ConfirmadoDisponible { get; set; }
    [StringLength(500)] public string? Observaciones { get; set; }
}

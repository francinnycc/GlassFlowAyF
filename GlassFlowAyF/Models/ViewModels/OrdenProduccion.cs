using System.ComponentModel.DataAnnotations;
namespace GlassFlowAyF.Models;

public class OrdenProduccion
{
    public int Id { get; set; }
    public int CompraId { get; set; }
    public Compra Compra { get; set; } = null!;
    [Required, StringLength(40)] public string NumeroOrden { get; set; } = string.Empty;
    [Required, StringLength(30)] public string Estado { get; set; } = "Pendiente";
    [DataType(DataType.Date)] public DateTime FechaInicioPrevista { get; set; }
    [DataType(DataType.Date)] public DateTime FechaFinPrevista { get; set; }
    public DateTime? FechaInicioReal { get; set; }
    public DateTime? FechaFinReal { get; set; }
    [StringLength(150)] public string? Responsable { get; set; }
    [StringLength(1000)] public string? Recursos { get; set; }
    [StringLength(2000)] public string? Observaciones { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public ICollection<OrdenProduccionMaterial> Materiales { get; set; } = new List<OrdenProduccionMaterial>();
}

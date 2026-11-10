using System.ComponentModel.DataAnnotations;
namespace GlassFlowAyF.ViewModels;

public class ProduccionFormViewModel
{
    [Required] public int CompraId { get; set; }
    [Required, DataType(DataType.Date), Display(Name = "Inicio previsto")] public DateTime FechaInicioPrevista { get; set; } = DateTime.Today;
    [Required, DataType(DataType.Date), Display(Name = "Finalización prevista")] public DateTime FechaFinPrevista { get; set; } = DateTime.Today.AddDays(7);
    [Required(ErrorMessage = "Asigná un responsable de producción."), StringLength(150)] public string? Responsable { get; set; }
    [StringLength(1000)] public string? Recursos { get; set; }
    [StringLength(2000)] public string? Observaciones { get; set; }
}
public class ProduccionMaterialViewModel
{
    [Required] public int OrdenId { get; set; }
    [Required] public int MaterialId { get; set; }
    public decimal Cantidad { get; set; } = 1;
    [Required, StringLength(30)] public string Unidad { get; set; } = "unidad";
    public bool ConfirmadoDisponible { get; set; }
    [StringLength(500)] public string? Observaciones { get; set; }
}

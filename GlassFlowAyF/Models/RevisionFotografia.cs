using System.ComponentModel.DataAnnotations;
namespace GlassFlowAyF.Models;

public class RevisionFotografia
{
    public int Id { get; set; }
    public int SolicitudFotografiaId { get; set; }
    public SolicitudFotografia? SolicitudFotografia { get; set; }
    [Required, StringLength(30)] public string Estado { get; set; } = "Pendiente";
    [StringLength(1500)] public string? Observaciones { get; set; }
    [StringLength(150)] public string Autor { get; set; } = "";
    public DateTime FechaRevision { get; set; } = DateTime.Now;
    [StringLength(5000)] public string? AnalisisIA { get; set; }
    [StringLength(100)] public string? ModeloIA { get; set; }
}

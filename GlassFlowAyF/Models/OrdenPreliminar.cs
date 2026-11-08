using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models;

/// <summary>Orden preliminar generada al aprobar la propuesta; habilita el flujo de producción e instalación.</summary>
public class OrdenPreliminar
{
    public int Id { get; set; }

    public int CotizacionId { get; set; }
    public Cotizacion? Cotizacion { get; set; }

    public int SolicitudCotizacionId { get; set; }
    public SolicitudCotizacion? SolicitudCotizacion { get; set; }

    [Required, StringLength(40)]
    public string Numero { get; set; } = string.Empty;

    [Required, StringLength(40)]
    public string Estado { get; set; } = "Preliminar";

    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}

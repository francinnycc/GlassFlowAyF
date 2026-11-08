using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models;

/// <summary>Notificación interna para el equipo (administrador, cotizador, operaciones).</summary>
public class Notificacion
{
    public int Id { get; set; }

    [Required, StringLength(40)]
    public string DestinatarioRol { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    public string Mensaje { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Url { get; set; }

    /// <summary>Aviso | Tarea</summary>
    [Required, StringLength(20)]
    public string Tipo { get; set; } = "Aviso";

    public int? SolicitudCotizacionId { get; set; }

    public bool Leida { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;
}

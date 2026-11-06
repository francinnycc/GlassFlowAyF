using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlassFlowAyF.Models
{
    /// <summary>
    /// Módulo 14 - Propuesta visual preliminar asociada a una solicitud de cotización.
    /// La IA y el cotizador solo preparan una representación preliminar; la decisión
    /// final del cliente (aprobar, rechazar o pedir cambios) se implementa en el módulo 15.
    /// </summary>
    public class PropuestaVisual
    {
        public int Id { get; set; }

        // Identificador único legible (PROP-VIS-001). Se asigna al guardar, cuando ya existe el Id.
        [StringLength(20)]
        [Display(Name = "Código")]
        public string? Codigo { get; set; }

        [Required]
        public int SolicitudCotizacionId { get; set; }
        public SolicitudCotizacion? SolicitudCotizacion { get; set; }

        // Opcional: diseño generado con IA que sirve de base (reutiliza el módulo existente).
        public int? DisenoIAId { get; set; }
        public DisenoIA? DisenoIA { get; set; }

        [Required(ErrorMessage = "Ingrese un título para la propuesta.")]
        [StringLength(120)]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese la descripción de la propuesta.")]
        [StringLength(2000)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Imagen principal")]
        public string? RutaImagenPrincipal { get; set; }

        // Estado propio de la propuesta. El módulo 15 lo ampliará
        // (Aprobada, Rechazada, Cambios solicitados).
        [Required]
        [StringLength(40)]
        public string Estado { get; set; } = "Propuesta Visual Creada";

        // Versionado visible: v1.0, v1.1, v1.2...
        public int VersionMayor { get; set; } = 1;
        public int VersionMenor { get; set; } = 0;

        [NotMapped]
        public string VersionTexto => $"v{VersionMayor}.{VersionMenor}";

        [Required]
        [StringLength(256)]
        public string CreadaPor { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaActualizacion { get; set; }

        // Evita que dos usuarios sobrescriban cambios al mismo tiempo (mismo patrón que Cotizacion).
        [ConcurrencyCheck]
        public Guid RowVersion { get; set; } = Guid.NewGuid();

        public ICollection<PropuestaVisualArchivo> Archivos { get; set; } = new List<PropuestaVisualArchivo>();
        public ICollection<PropuestaVisualBitacora> Bitacora { get; set; } = new List<PropuestaVisualBitacora>();
    }
}

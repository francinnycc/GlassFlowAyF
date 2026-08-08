using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models
{
    public class TrabajoInstalacion
    {
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Cliente { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Producto { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string Direccion { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Fecha de instalación")]
        public DateTime FechaInstalacion { get; set; }

        [Required]
        [StringLength(120)]
        public string Instalador { get; set; } = string.Empty;

        [StringLength(40)]
        public string Estado { get; set; } = "Programada";

        [Display(Name = "Medidas confirmadas")]
        public bool MedidasConfirmadas { get; set; }

        [Display(Name = "Material recibido")]
        public bool MaterialListo { get; set; }

        [Display(Name = "Instalación realizada")]
        public bool InstalacionRealizada { get; set; }

        [Display(Name = "Revisión de acabados")]
        public bool RevisionAcabados { get; set; }

        [Display(Name = "Limpieza final")]
        public bool LimpiezaFinal { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}
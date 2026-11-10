using System;
using System.ComponentModel.DataAnnotations;
namespace GlassFlowAyF.Models.ViewModels
{
    public class ReasignarInstalacionViewModel
    {
        public int Id { get; set; }
        public string InstaladorActual { get; set; } = "";
        [Required(ErrorMessage = "Seleccione un instalador.")]
        public string InstaladorId { get; set; } = "";
        [Required]
        [Display(Name = "Fecha de instalación")]
        public DateTime FechaInstalacion { get; set; }
        [Required(ErrorMessage = "Indique el motivo.")]
        [StringLength(250)]
        public string Motivo { get; set; } = "";
    }
}

using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models.ViewModels
{
    public class GenerarDisenoIAViewModel
    {
        [Required]
        public int SolicitudId { get; set; }

        [Display(Name = "Fotografía del espacio")]
        public int? FotografiaBaseId { get; set; }

        [Required(ErrorMessage = "Seleccione un estilo.")]
        [StringLength(60)]
        public string Estilo { get; set; }
            = "Moderno";

        [Display(Name = "Color de perfilería")]
        [StringLength(60)]
        public string ColorPerfil { get; set; }
            = "Negro mate";

        [Display(Name = "Ambiente deseado")]
        [StringLength(80)]
        public string Ambiente { get; set; }
            = "Elegante y luminoso";

        [Display(Name = "Preferencias adicionales")]
        [StringLength(1000)]
        public string? Preferencias { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models.ViewModels
{
    public class PropuestaVisualFormViewModel
    {
        public int Id { get; set; }

        public int SolicitudId { get; set; }

        [Required(ErrorMessage = "Ingrese un título para la propuesta.")]
        [StringLength(120)]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese la descripción de la propuesta.")]
        [StringLength(2000)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Diseño generado con IA (opcional)")]
        public int? DisenoIAId { get; set; }

        // Control de concurrencia al editar.
        public Guid RowVersion { get; set; }

        [Display(Name = "Archivos adjuntos")]
        public List<IFormFile>? Archivos { get; set; }
    }
}

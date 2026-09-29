using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models.ViewModels
{
    public class CrearSoporteViewModel
    {
        [Required(ErrorMessage = "Seleccione el tipo de solicitud.")]
        [Display(Name = "Tipo de solicitud")]
        public string Tipo { get; set; } = "Consulta";

        [Required(ErrorMessage = "Seleccione una categoría.")]
        [Display(Name = "Categoría")]
        public string Categoria { get; set; } = string.Empty;

        [Required(ErrorMessage = "El asunto es obligatorio.")]
        [StringLength(
            150,
            ErrorMessage = "El asunto no puede superar los 150 caracteres.")]
        [Display(Name = "Asunto")]
        public string Asunto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El mensaje es obligatorio.")]
        [StringLength(
            2000,
            MinimumLength = 10,
            ErrorMessage = "El mensaje debe tener entre 10 y 2000 caracteres.")]
        [Display(Name = "Mensaje")]
        public string Mensaje { get; set; } = string.Empty;
    }
}
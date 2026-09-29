using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models.ViewModels
{
    public class ResponderSoporteViewModel
    {
        public int Id { get; set; }


        public string Cliente { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public string Asunto { get; set; } = string.Empty;

        public string Mensaje { get; set; } = string.Empty;


        [Required(ErrorMessage = "La respuesta es obligatoria.")]
        [StringLength(
            3000,
            MinimumLength = 5,
            ErrorMessage = "La respuesta debe tener entre 5 y 3000 caracteres.")]
        [Display(Name = "Respuesta")]
        public string Respuesta { get; set; } = string.Empty;


        [Required]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Respondido";
    }
}
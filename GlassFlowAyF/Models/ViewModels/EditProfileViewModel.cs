using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models.ViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(
            150,
            ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; } = string.Empty;


        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [Phone(ErrorMessage = "Ingrese un número de teléfono válido.")]
        [StringLength(
            30,
            ErrorMessage = "El teléfono no puede superar los 30 caracteres.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;


        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(
            300,
            ErrorMessage = "La dirección no puede superar los 300 caracteres.")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;
    }
}

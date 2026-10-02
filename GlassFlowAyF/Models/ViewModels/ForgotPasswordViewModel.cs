using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage =
            "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage =
            "Ingrese un correo electrónico válido.")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
            = string.Empty;
    }
}
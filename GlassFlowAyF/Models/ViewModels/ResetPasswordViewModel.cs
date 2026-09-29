using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
            = string.Empty;


        [Required]
        public string Token { get; set; }
            = string.Empty;


        [Required(ErrorMessage =
            "La nueva contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [MinLength(
            6,
            ErrorMessage =
                "La contraseña debe tener al menos 6 caracteres.")]
        [Display(Name = "Nueva contraseña")]
        public string Password { get; set; }
            = string.Empty;


        [Required(ErrorMessage =
            "Debe confirmar la contraseña.")]
        [DataType(DataType.Password)]
        [Compare(
            nameof(Password),
            ErrorMessage =
                "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmPassword { get; set; }
            = string.Empty;
    }
}
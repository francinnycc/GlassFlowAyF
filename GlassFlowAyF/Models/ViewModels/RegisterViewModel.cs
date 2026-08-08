using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage =
            "El nombre es obligatorio.")]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; }
            = string.Empty;

        [Required(ErrorMessage =
            "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage =
            "Ingrese un correo válido.")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
            = string.Empty;

        [Required(ErrorMessage =
            "El teléfono es obligatorio.")]
        [Phone]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }
            = string.Empty;

        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [Required(ErrorMessage =
            "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [MinLength(
            6,
            ErrorMessage =
                "La contraseña debe tener al menos 6 caracteres.")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
            = string.Empty;

        [Required]
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
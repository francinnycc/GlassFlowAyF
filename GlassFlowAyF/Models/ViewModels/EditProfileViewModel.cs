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


        [Required(ErrorMessage = "La cédula es obligatoria.")]
        [StringLength(
            15,
            ErrorMessage = "La cédula no puede superar los 15 caracteres.")]
        [RegularExpression(
            @"^\d{9}$|^\d{3}-\d{3}-\d{3}$",
            ErrorMessage = "La cédula debe tener 9 dígitos. Ejemplo: 123456789 o 123-456-789.")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = string.Empty;


        [Required(ErrorMessage = "El número de contacto de emergencia es obligatorio.")]
        [Phone(ErrorMessage = "Ingrese un número de teléfono válido.")]
        [StringLength(
            30,
            ErrorMessage = "El número de contacto no puede superar los 30 caracteres.")]
        [Display(Name = "Número de contacto de emergencia")]
        public string NumeroContactoEmergencia
        { get; set; } = string.Empty;


        [StringLength(
            1000,
            ErrorMessage = "Las alergias o medicamentos no pueden superar los 1000 caracteres.")]
        [Display(Name = "Alergias o medicamentos (si posee)")]
        public string? AlergiasMedicamentos
        { get; set; }


        [StringLength(
            1000,
            ErrorMessage = "Los padecimientos o enfermedades no pueden superar los 1000 caracteres.")]
        [Display(Name = "Padecimientos o enfermedades (si posee)")]
        public string? PadecimientosEnfermedades
        { get; set; }


        [StringLength(
            1000,
            ErrorMessage = "Los medicamentos no pueden superar los 1000 caracteres.")]
        [Display(Name = "Medicamentos (si toma alguno)")]
        public string? Medicamentos { get; set; }
    }
}

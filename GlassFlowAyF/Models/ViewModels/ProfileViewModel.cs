using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;

        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; }

        public string Rol { get; set; } = string.Empty;
    }
}
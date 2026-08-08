using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(120)]
        public string NombreCompleto { get; set; }
            = string.Empty;

        [StringLength(300)]
        public string? Direccion { get; set; }

        public DateTime FechaRegistro { get; set; }
            = DateTime.Now;

        public bool Activo { get; set; } = true;
    }
}
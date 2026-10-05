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


        // ============================================
        // DATOS PERSONALES Y CONTACTO DE EMERGENCIA
        // ============================================

        [Required]
        [StringLength(15)]
        public string Cedula { get; set; }
            = string.Empty;


        [StringLength(30)]
        public string? NumeroContactoEmergencia
        { get; set; }


        // ============================================
        // INFORMACIÓN DE SALUD
        // ============================================

        [StringLength(1000)]
        public string? AlergiasMedicamentos
        { get; set; }


        [StringLength(1000)]
        public string? PadecimientosEnfermedades
        { get; set; }


        [StringLength(1000)]
        public string? Medicamentos { get; set; }


        public ICollection<TrabajoInstalacion>
            TrabajosAsignados
        { get; set; }
            = new List<TrabajoInstalacion>();


        public ICollection<Compra>
            Compras
        { get; set; }
            = new List<Compra>();


        // ============================================
        // SOPORTE
        // ============================================

        public ICollection<SoporteSolicitud>
            SolicitudesSoporte
        { get; set; }
            = new List<SoporteSolicitud>();
    }
}
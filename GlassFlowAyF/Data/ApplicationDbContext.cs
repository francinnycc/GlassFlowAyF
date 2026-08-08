using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos => Set<Producto>();

        public DbSet<SolicitudCotizacion> SolicitudesCotizacion
            => Set<SolicitudCotizacion>();

        public DbSet<VisitaTecnica> VisitasTecnicas
            => Set<VisitaTecnica>();

        public DbSet<TrabajoInstalacion> TrabajosInstalacion
            => Set<TrabajoInstalacion>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>().HasData(

                new Producto
                {
                    Id = 1,
                    Nombre = "Puerta de baño corrediza",
                    Categoria = "Puertas",
                    Descripcion =
                        "Puerta corrediza en vidrio templado.",
                    PrecioBase = 350000m,
                    Activo = true
                },

                new Producto
                {
                    Id = 2,
                    Nombre = "Ventana en vidrio",
                    Categoria = "Ventanas",
                    Descripcion =
                        "Ventana personalizada en aluminio y vidrio.",
                    PrecioBase = 180000m,
                    Activo = true
                },

                new Producto
                {
                    Id = 3,
                    Nombre = "Espejo personalizado",
                    Categoria = "Espejos",
                    Descripcion =
                        "Espejo fabricado según medidas del cliente.",
                    PrecioBase = 95000m,
                    Activo = true
                },

                new Producto
                {
                    Id = 4,
                    Nombre = "Estructura en vidrio",
                    Categoria = "Estructuras",
                    Descripcion =
                        "Estructura personalizada para interiores.",
                    PrecioBase = 420000m,
                    Activo = true
                }
            );
        }
    }
}
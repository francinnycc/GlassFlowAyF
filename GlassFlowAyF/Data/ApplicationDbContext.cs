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

        public DbSet<Producto> Productos
            => Set<Producto>();

        public DbSet<Material> Materiales
            => Set<Material>();

        public DbSet<ProductoMaterial> ProductoMateriales
            => Set<ProductoMaterial>();

        public DbSet<SolicitudCotizacion> SolicitudesCotizacion
            => Set<SolicitudCotizacion>();

        public DbSet<SolicitudFotografia> SolicitudFotografias
            => Set<SolicitudFotografia>();

        public DbSet<VisitaTecnica> VisitasTecnicas
            => Set<VisitaTecnica>();

        public DbSet<TrabajoInstalacion> TrabajosInstalacion
            => Set<TrabajoInstalacion>();

        public DbSet<Cotizacion> Cotizaciones
            => Set<Cotizacion>();

        public DbSet<Compra> Compras
            => Set<Compra>();

        public DbSet<Factura> Facturas
            => Set<Factura>();


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // PRODUCTO - MATERIAL

            modelBuilder.Entity<ProductoMaterial>()
                .HasKey(pm => new
                {
                    pm.ProductoId,
                    pm.MaterialId
                });

            modelBuilder.Entity<ProductoMaterial>()
                .HasOne(pm => pm.Producto)
                .WithMany(p => p.ProductoMateriales)
                .HasForeignKey(pm => pm.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductoMaterial>()
                .HasOne(pm => pm.Material)
                .WithMany(m => m.ProductoMateriales)
                .HasForeignKey(pm => pm.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);


            // SOLICITUD - PRODUCTO

            modelBuilder.Entity<SolicitudCotizacion>()
                .HasOne(s => s.Producto)
                .WithMany(p => p.Solicitudes)
                .HasForeignKey(s => s.ProductoId)
                .OnDelete(DeleteBehavior.SetNull);


            // SOLICITUD - MATERIAL

            modelBuilder.Entity<SolicitudCotizacion>()
                .HasOne(s => s.Material)
                .WithMany(m => m.Solicitudes)
                .HasForeignKey(s => s.MaterialId)
                .OnDelete(DeleteBehavior.SetNull);


            // SOLICITUD - FOTOGRAFÍAS

            modelBuilder.Entity<SolicitudFotografia>()
                .HasOne(f => f.SolicitudCotizacion)
                .WithMany(s => s.Fotografias)
                .HasForeignKey(f => f.SolicitudCotizacionId)
                .OnDelete(DeleteBehavior.Cascade);


            // SOLICITUD - COTIZACIONES

            modelBuilder.Entity<Cotizacion>()
                .HasOne(c => c.SolicitudCotizacion)
                .WithMany(s => s.Cotizaciones)
                .HasForeignKey(c => c.SolicitudCotizacionId)
                .OnDelete(DeleteBehavior.Cascade);


            // COTIZACIÓN - COMPRA 1:1

            modelBuilder.Entity<Compra>()
                .HasOne(c => c.Cotizacion)
                .WithOne(c => c.Compra)
                .HasForeignKey<Compra>(c => c.CotizacionId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Compra>()
                .HasIndex(c => c.CotizacionId)
                .IsUnique();


            // CLIENTE - COMPRAS

            modelBuilder.Entity<Compra>()
                .HasOne(c => c.Cliente)
                .WithMany(u => u.Compras)
                .HasForeignKey(c => c.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);


            // COMPRA - FACTURA 1:1

            modelBuilder.Entity<Factura>()
                .HasOne(f => f.Compra)
                .WithOne(c => c.Factura)
                .HasForeignKey<Factura>(f => f.CompraId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Factura>()
                .HasIndex(f => f.CompraId)
                .IsUnique();


            // INSTALADOR - TRABAJOS

            modelBuilder.Entity<TrabajoInstalacion>()
                .HasOne(t => t.Instalador)
                .WithMany(u => u.TrabajosAsignados)
                .HasForeignKey(t => t.InstaladorId)
                .OnDelete(DeleteBehavior.SetNull);


            // COMPRA - INSTALACIÓN

            modelBuilder.Entity<TrabajoInstalacion>()
                .HasOne(t => t.Compra)
                .WithOne(c => c.TrabajoInstalacion)
                .HasForeignKey<TrabajoInstalacion>(
                    t => t.CompraId)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<TrabajoInstalacion>()
                .HasOne(t => t.SolicitudCotizacion)
                .WithMany()
                .HasForeignKey(t =>
                    t.SolicitudCotizacionId)
                .OnDelete(DeleteBehavior.SetNull);


            // =============================
            // PRODUCTOS DEMO
            // =============================

            modelBuilder.Entity<Producto>().HasData(

                new Producto
                {
                    Id = 1,
                    Nombre = "Puerta de baño corrediza",
                    Categoria = "Divisiones de baño",
                    Descripcion =
                        "Sistema corredizo en vidrio templado para baño.",
                    PrecioBase = 185000m,
                    Activo = true,
                    PermiteInstalacion = true,
                    FechaCreacion = new DateTime(2026, 1, 1)
                },

                new Producto
                {
                    Id = 2,
                    Nombre = "Ventana corrediza en aluminio y vidrio",
                    Categoria = "Ventanas",
                    Descripcion =
                        "Ventana corrediza fabricada a medida.",
                    PrecioBase = 95000m,
                    Activo = true,
                    PermiteInstalacion = true,
                    FechaCreacion = new DateTime(2026, 1, 1)
                },

                new Producto
                {
                    Id = 3,
                    Nombre = "Espejo personalizado",
                    Categoria = "Espejos",
                    Descripcion =
                        "Espejo elaborado según las medidas del espacio.",
                    PrecioBase = 65000m,
                    Activo = true,
                    PermiteInstalacion = true,
                    FechaCreacion = new DateTime(2026, 1, 1)
                },

                new Producto
                {
                    Id = 4,
                    Nombre = "Baranda de vidrio templado",
                    Categoria = "Barandas",
                    Descripcion =
                        "Baranda moderna en vidrio templado de seguridad.",
                    PrecioBase = 190000m,
                    Activo = true,
                    PermiteInstalacion = true,
                    FechaCreacion = new DateTime(2026, 1, 1)
                }
            );


            // =============================
            // MATERIALES DEMO
            // =============================

            modelBuilder.Entity<Material>().HasData(

                new Material
                {
                    Id = 1,
                    Nombre = "Vidrio templado claro 10 mm",
                    Tipo = "Vidrio templado",
                    Color = "Transparente",
                    Perfil = "Aluminio negro",
                    Acabado = "Brillante",
                    Grosor = 10m,
                    PrecioAdicional = 0m,
                    Activo = true
                },

                new Material
                {
                    Id = 2,
                    Nombre = "Vidrio templado bronce 10 mm",
                    Tipo = "Vidrio templado",
                    Color = "Bronce",
                    Perfil = "Aluminio negro",
                    Acabado = "Brillante",
                    Grosor = 10m,
                    PrecioAdicional = 25000m,
                    Activo = true
                },

                new Material
                {
                    Id = 3,
                    Nombre = "Vidrio laminado 12 mm",
                    Tipo = "Vidrio laminado",
                    Color = "Transparente",
                    Perfil = "Aluminio natural",
                    Acabado = "Natural",
                    Grosor = 12m,
                    PrecioAdicional = 35000m,
                    Activo = true
                },

                new Material
                {
                    Id = 4,
                    Nombre = "Vidrio esmerilado 8 mm",
                    Tipo = "Vidrio templado",
                    Color = "Esmerilado",
                    Perfil = "Aluminio negro",
                    Acabado = "Mate",
                    Grosor = 8m,
                    PrecioAdicional = 30000m,
                    Activo = true
                }
            );


            modelBuilder.Entity<ProductoMaterial>().HasData(

                new ProductoMaterial { ProductoId = 1, MaterialId = 1 },
                new ProductoMaterial { ProductoId = 1, MaterialId = 2 },
                new ProductoMaterial { ProductoId = 1, MaterialId = 4 },

                new ProductoMaterial { ProductoId = 2, MaterialId = 1 },
                new ProductoMaterial { ProductoId = 2, MaterialId = 2 },
                new ProductoMaterial { ProductoId = 2, MaterialId = 3 },

                new ProductoMaterial { ProductoId = 3, MaterialId = 1 },
                new ProductoMaterial { ProductoId = 3, MaterialId = 4 },

                new ProductoMaterial { ProductoId = 4, MaterialId = 1 },
                new ProductoMaterial { ProductoId = 4, MaterialId = 3 }
            );
        }
    }
}
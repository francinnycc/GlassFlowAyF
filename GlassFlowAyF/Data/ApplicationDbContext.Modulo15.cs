using GlassFlowAyF.Models;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Data;

// Módulo 15 – Revisión y Aprobación del Cliente
public partial class ApplicationDbContext
{
    public DbSet<RevisionCliente> RevisionesCliente => Set<RevisionCliente>();
    public DbSet<OrdenPreliminar> OrdenesPreliminares => Set<OrdenPreliminar>();
    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();

    private static void ConfigurarModulo15(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cotizacion>()
            .HasOne(c => c.DisenoIA).WithMany()
            .HasForeignKey(c => c.DisenoIAId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<RevisionCliente>(e =>
        {
            e.HasOne(r => r.Cotizacion).WithMany(c => c.Revisiones)
                .HasForeignKey(r => r.CotizacionId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.SolicitudCotizacion).WithMany()
                .HasForeignKey(r => r.SolicitudCotizacionId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.DisenoIA).WithMany()
                .HasForeignKey(r => r.DisenoIAId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(r => r.Usuario).WithMany()
                .HasForeignKey(r => r.UsuarioId).OnDelete(DeleteBehavior.Restrict);
            // Una versión de la cotización solo puede recibir una decisión (anti-duplicados).
            e.HasIndex(r => new { r.CotizacionId, r.VersionCotizacion }).IsUnique();
            e.HasIndex(r => r.FechaUtc);
        });

        modelBuilder.Entity<OrdenPreliminar>(e =>
        {
            e.HasOne(o => o.Cotizacion).WithOne(c => c.OrdenPreliminar)
                .HasForeignKey<OrdenPreliminar>(o => o.CotizacionId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(o => o.SolicitudCotizacion).WithMany()
                .HasForeignKey(o => o.SolicitudCotizacionId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(o => o.Numero).IsUnique();
        });

        modelBuilder.Entity<Notificacion>()
            .HasIndex(n => new { n.DestinatarioRol, n.Leida });
    }
}

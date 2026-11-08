using GlassFlowAyF.Models;
using GlassFlowAyF.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GlassFlowAyF.Tests;

/// <summary>Pruebas unitarias del Módulo 15 sobre el servicio (HU 1, HU 2 y HU 3).</summary>
public class RevisionClienteServiceTests : IDisposable
{
    private readonly TestDb _db = new();
    public void Dispose() => _db.Dispose();

    // ---------------------------------------------------------------- HU 1

    [Fact] // 1.1
    public async Task Consulta_con_cotizacion_y_diseno_devuelve_panel_integrado()
    {
        using var ctx = _db.NuevoContexto();
        var r = await _db.Servicio(ctx).ObtenerRevisionAsync(_db.SolicitudId, _db.ClienteId, esAdministrador: false);

        Assert.Equal(CodigoRevision.Ok, r.Codigo);
        Assert.NotNull(r.Modelo!.Cotizacion);
        Assert.Equal(900, r.Modelo.Cotizacion!.Total);
        Assert.Equal(_db.DisenoId, r.Modelo.Diseno!.Id);
        Assert.True(r.Modelo.PuedeResponder);
    }

    [Fact] // 1.2
    public async Task Consulta_sin_propuesta_devuelve_empty_state()
    {
        using var ctx = _db.NuevoContexto();
        var r = await _db.Servicio(ctx).ObtenerRevisionAsync(_db.SolicitudSinPropuestaId, _db.ClienteId, false);

        Assert.Equal(CodigoRevision.Ok, r.Codigo);
        Assert.Null(r.Modelo!.Cotizacion);
        Assert.False(r.Modelo.PuedeResponder);
    }

    [Fact] // 1.3
    public async Task Detalle_incluye_material_perfileria_y_renders()
    {
        using var ctx = _db.NuevoContexto();
        var r = await _db.Servicio(ctx).ObtenerRevisionAsync(_db.SolicitudId, _db.ClienteId, false, _db.CotizacionId);

        var s = r.Modelo!.Solicitud;
        Assert.Equal("Vidrio templado", s.Material!.Tipo);
        Assert.Equal("Aluminio serie 80", s.Material.Perfil);
        Assert.Single(s.DisenosIA);
        Assert.Equal("50% de adelanto.", r.Modelo.Cotizacion!.Condiciones);
    }

    [Fact]
    public async Task Consulta_de_solicitud_ajena_es_prohibida_y_admin_puede_consultar()
    {
        using var ctx = _db.NuevoContexto();
        var servicio = _db.Servicio(ctx);

        Assert.Equal(CodigoRevision.Prohibido,
            (await servicio.ObtenerRevisionAsync(_db.SolicitudId, _db.OtroClienteId, false)).Codigo);

        var admin = await servicio.ObtenerRevisionAsync(_db.SolicitudId, _db.AdminId, esAdministrador: true);
        Assert.Equal(CodigoRevision.Ok, admin.Codigo);
        Assert.False(admin.Modelo!.PuedeResponder); // el administrador consulta, no decide
    }

    // ---------------------------------------------------------------- HU 2

    [Fact] // 2.1 + 2.2
    public async Task Aprobar_congela_cambia_estados_genera_orden_y_audita()
    {
        using (var ctx = _db.NuevoContexto())
        {
            var r = await _db.Servicio(ctx).AprobarAsync(_db.Comando(ctx));
            Assert.True(r.Ok, r.Mensaje);
            Assert.NotNull(r.OrdenId);
        }

        using var verif = _db.NuevoContexto();
        var cot = await verif.Cotizaciones.Include(c => c.SolicitudCotizacion).SingleAsync(c => c.Id == _db.CotizacionId);
        Assert.Equal(EstadosRevision.CotAprobada, cot.Estado);
        Assert.True(cot.Congelada);
        Assert.NotNull(cot.FechaCongelacion);
        Assert.Equal(64, cot.SnapshotHash!.Length);
        Assert.Equal(_db.DisenoId, cot.DisenoIAId);
        Assert.Equal(EstadosRevision.AprobadoCliente, cot.SolicitudCotizacion!.Estado);

        var orden = await verif.OrdenesPreliminares.SingleAsync();
        Assert.StartsWith("GF-OP-", orden.Numero);

        var log = await verif.RevisionesCliente.SingleAsync();
        Assert.Equal(EstadosRevision.DecisionAprobada, log.Decision);
        Assert.Equal("203.0.113.10", log.DireccionIp);
        Assert.Equal(_db.ClienteId, log.UsuarioId);
        Assert.True(log.AceptoTerminos);
        Assert.True((DateTime.UtcNow - log.FechaUtc).TotalMinutes < 1);
        Assert.Equal(EstadosRevision.CotPendiente, log.EstadoCotizacionAnterior);
        Assert.Contains("\"Total\":900", log.SnapshotJson);
    }

    [Fact] // 2.2
    public async Task Aprobar_sin_aceptar_terminos_no_cambia_nada()
    {
        using var ctx = _db.NuevoContexto();
        var r = await _db.Servicio(ctx).AprobarAsync(_db.Comando(ctx, terminos: false));

        Assert.Equal(CodigoRevision.Invalido, r.Codigo);
        Assert.Equal(EstadosRevision.CotPendiente, (await ctx.Cotizaciones.AsNoTracking().SingleAsync()).Estado);
        Assert.Empty(ctx.RevisionesCliente);
        Assert.Empty(ctx.OrdenesPreliminares);
    }

    [Fact] // control de duplicados
    public async Task Doble_aprobacion_con_la_misma_version_se_rechaza()
    {
        ComandoRevision comando;
        using (var ctx = _db.NuevoContexto())
        {
            comando = _db.Comando(ctx);
            Assert.True((await _db.Servicio(ctx).AprobarAsync(comando)).Ok);
        }

        using (var ctx = _db.NuevoContexto())
        {
            var segunda = await _db.Servicio(ctx).AprobarAsync(comando); // pestaña vieja / doble clic
            Assert.Equal(CodigoRevision.Conflicto, segunda.Codigo);
        }

        using var verif = _db.NuevoContexto();
        Assert.Equal(1, await verif.RevisionesCliente.CountAsync());
        Assert.Equal(1, await verif.OrdenesPreliminares.CountAsync());
    }

    [Fact] // control de duplicados a nivel de BD
    public async Task Indice_unico_impide_dos_decisiones_para_la_misma_version()
    {
        using var ctx = _db.NuevoContexto();
        var version = Guid.NewGuid();
        RevisionCliente Nueva() => new()
        {
            CotizacionId = _db.CotizacionId, SolicitudCotizacionId = _db.SolicitudId,
            UsuarioId = _db.ClienteId, Decision = "Aprobada", VersionCotizacion = version
        };

        ctx.RevisionesCliente.Add(Nueva());
        await ctx.SaveChangesAsync();
        ctx.RevisionesCliente.Add(Nueva());
        await Assert.ThrowsAsync<DbUpdateException>(() => ctx.SaveChangesAsync());
    }

    [Fact]
    public async Task No_se_aprueba_cotizacion_vencida_ni_ajena()
    {
        using (var ctx = _db.NuevoContexto())
        {
            var ajena = await _db.Servicio(ctx).AprobarAsync(_db.Comando(ctx, usuarioId: _db.OtroClienteId));
            Assert.Equal(CodigoRevision.Prohibido, ajena.Codigo);
        }

        using (var ctx = _db.NuevoContexto())
        {
            var cot = await ctx.Cotizaciones.SingleAsync();
            cot.FechaVencimiento = DateTime.Today.AddDays(-1);
            await ctx.SaveChangesAsync();
            var vencida = await _db.Servicio(ctx).AprobarAsync(_db.Comando(ctx));
            Assert.Equal(CodigoRevision.Invalido, vencida.Codigo);
        }

        using var verif = _db.NuevoContexto();
        Assert.Empty(verif.RevisionesCliente);
    }

    [Fact] // 2.3
    public async Task Aprobacion_notifica_a_admin_cotizador_operaciones_y_cliente()
    {
        using var ctx = _db.NuevoContexto();
        Assert.True((await _db.Servicio(ctx).AprobarAsync(_db.Comando(ctx))).Ok);

        var roles = await ctx.Notificaciones.Select(n => n.DestinatarioRol).ToListAsync();
        Assert.Contains(EstadosRevision.RolAdministrador, roles);
        Assert.Contains(EstadosRevision.RolCotizador, roles);
        Assert.Contains(EstadosRevision.RolOperaciones, roles);

        var destinos = _db.Email.Enviados.Select(e => e.Destino).ToList();
        Assert.Contains(TestDb.CorreoAdmin, destinos);
        Assert.Contains(TestDb.CorreoCotizador, destinos);
        Assert.Contains(TestDb.CorreoOperaciones, destinos);
        Assert.Contains(TestDb.CorreoCliente, destinos);
        Assert.Contains(_db.Email.Enviados, e => e.Destino == TestDb.CorreoCliente && e.Cuerpo.Contains("Siguientes pasos"));
    }

    [Fact] // 2.3 – robustez
    public async Task Fallo_de_correo_no_revierte_la_aprobacion()
    {
        _db.Email.Fallar = true;
        using var ctx = _db.NuevoContexto();
        var r = await _db.Servicio(ctx).AprobarAsync(_db.Comando(ctx));

        Assert.True(r.Ok);
        using var verif = _db.NuevoContexto();
        Assert.True((await verif.Cotizaciones.SingleAsync()).Congelada);
    }

    // ---------------------------------------------------------------- HU 3

    [Fact] // 3.1
    public async Task Solicitar_modificacion_guarda_observaciones_reasigna_y_bloquea_aprobacion()
    {
        const string cambios = "Cambiar la perfilería a color blanco y agregar toallero.";
        using (var ctx = _db.NuevoContexto())
        {
            var r = await _db.Servicio(ctx).SolicitarModificacionAsync(_db.Comando(ctx, cambios));
            Assert.True(r.Ok, r.Mensaje);
        }

        using (var ctx = _db.NuevoContexto())
        {
            var cot = await ctx.Cotizaciones.Include(c => c.SolicitudCotizacion).SingleAsync();
            Assert.Equal(EstadosRevision.CotCambios, cot.Estado);
            Assert.Equal(cambios, cot.ComentarioCliente);
            Assert.Equal(EstadosRevision.SolicitudModificacion, cot.SolicitudCotizacion!.Estado);
            Assert.Contains(ctx.Notificaciones, n => n.DestinatarioRol == EstadosRevision.RolCotizador && n.Tipo == "Tarea");

            // La aprobación directa queda deshabilitada hasta que el cotizador publique otra versión.
            var aprobar = await _db.Servicio(ctx).AprobarAsync(_db.Comando(ctx));
            Assert.Equal(CodigoRevision.Conflicto, aprobar.Codigo);
        }
    }

    [Fact] // 3.2
    public async Task Rechazar_registra_motivo_y_desactiva_propuesta()
    {
        const string motivo = "El presupuesto supera lo planificado para este año.";
        using (var ctx = _db.NuevoContexto())
            Assert.True((await _db.Servicio(ctx).RechazarAsync(_db.Comando(ctx, motivo))).Ok);

        using var verif = _db.NuevoContexto();
        var cot = await verif.Cotizaciones.Include(c => c.SolicitudCotizacion).SingleAsync();
        Assert.Equal(EstadosRevision.CotRechazada, cot.Estado);
        Assert.False(cot.Activa);
        Assert.Equal(EstadosRevision.RechazadaCliente, cot.SolicitudCotizacion!.Estado);
        var log = await verif.RevisionesCliente.SingleAsync();
        Assert.Equal(EstadosRevision.DecisionRechazada, log.Decision);
        Assert.Equal(motivo, log.Comentario);
    }

    [Theory] // 3.3
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    [InlineData("corto")]
    public async Task Modificar_o_rechazar_sin_justificacion_se_bloquea(string? comentario)
    {
        using var ctx = _db.NuevoContexto();
        var servicio = _db.Servicio(ctx);

        var mod = await servicio.SolicitarModificacionAsync(_db.Comando(ctx, comentario));
        var rech = await servicio.RechazarAsync(_db.Comando(ctx, comentario));

        Assert.Equal(CodigoRevision.Invalido, mod.Codigo);
        Assert.Equal(EstadosRevision.MensajeComentarioObligatorio, mod.Mensaje);
        Assert.Equal(EstadosRevision.MensajeComentarioObligatorio, rech.Mensaje);
        Assert.Empty(ctx.RevisionesCliente);
        Assert.Equal(EstadosRevision.CotPendiente, (await ctx.Cotizaciones.AsNoTracking().SingleAsync()).Estado);
    }

    [Fact]
    public void Estado_esperado_segun_cotizaciones()
    {
        Assert.Null(EstadosRevision.EstadoSegunCotizaciones(Array.Empty<Cotizacion>()));
        Assert.Equal(EstadosRevision.SolicitudModificacion, EstadosRevision.EstadoSegunCotizaciones(
            new[] { new Cotizacion { Id = 1, Estado = EstadosRevision.CotCambios } }));
        Assert.Equal(EstadosRevision.AprobadoCliente, EstadosRevision.EstadoSegunCotizaciones(new[]
        {
            new Cotizacion { Id = 1, Estado = EstadosRevision.CotAprobada, FechaCreacion = DateTime.Today.AddDays(-1) },
            new Cotizacion { Id = 2, Estado = EstadosRevision.CotRechazada, FechaCreacion = DateTime.Today }
        }));
    }
}

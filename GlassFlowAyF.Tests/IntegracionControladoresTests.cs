using System.Reflection;
using GlassFlowAyF.Controllers;
using GlassFlowAyF.Models;
using GlassFlowAyF.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GlassFlowAyF.Tests;

/// <summary>Pruebas de integración: controladores + persistencia real (SQLite).</summary>
public class IntegracionControladoresTests : IDisposable
{
    private readonly TestDb _db = new();
    public void Dispose() => _db.Dispose();

    private RevisionClienteController Revision(Data.ApplicationDbContext ctx, string usuarioId, string rol) =>
        TestDb.ConUsuario(new RevisionClienteController(_db.Servicio(ctx), ctx), usuarioId, rol);

    private async Task AprobarComoClienteAsync()
    {
        using var ctx = _db.NuevoContexto();
        var version = (await ctx.Cotizaciones.AsNoTracking().SingleAsync()).Version;
        var r = await Revision(ctx, _db.ClienteId, "Cliente").Aprobar(_db.CotizacionId, version, aceptoTerminos: true);
        Assert.IsType<RedirectToActionResult>(r);
    }

    [Fact]
    public async Task Cliente_aprueba_desde_el_controlador_y_se_registra_la_ip_del_request()
    {
        await AprobarComoClienteAsync();

        using var verif = _db.NuevoContexto();
        var log = await verif.RevisionesCliente.SingleAsync();
        Assert.Equal("198.51.100.7", log.DireccionIp);
        Assert.Equal("Navegador-Pruebas/1.0", log.UserAgent);
    }

    [Fact]
    public async Task Rechazo_vacio_desde_controlador_deja_error_y_no_cambia_estado()
    {
        using var ctx = _db.NuevoContexto();
        var version = (await ctx.Cotizaciones.AsNoTracking().SingleAsync()).Version;
        var controller = Revision(ctx, _db.ClienteId, "Cliente");

        var r = await controller.Rechazar(_db.CotizacionId, version, "   ");

        var redirect = Assert.IsType<RedirectToActionResult>(r);
        Assert.Equal(nameof(RevisionClienteController.Revisar), redirect.ActionName);
        Assert.Equal(EstadosRevision.MensajeComentarioObligatorio, controller.TempData["Error"]);
        Assert.Empty(ctx.RevisionesCliente);
    }

    [Fact]
    public async Task Admin_consulta_panel_e_historial_de_cualquier_solicitud()
    {
        await AprobarComoClienteAsync();

        using var ctx = _db.NuevoContexto();
        var controller = Revision(ctx, _db.AdminId, "Administrador");

        var panel = Assert.IsType<ViewResult>(await controller.Revisar(_db.SolicitudId));
        Assert.IsType<RevisionPropuestaViewModel>(panel.Model);

        var historial = Assert.IsType<ViewResult>(await controller.Historial(_db.SolicitudId));
        var revisiones = Assert.IsType<List<RevisionCliente>>(historial.Model);
        Assert.Single(revisiones);
    }

    [Fact]
    public async Task Otro_cliente_no_puede_ver_la_propuesta_ajena()
    {
        using var ctx = _db.NuevoContexto();
        var r = await Revision(ctx, _db.OtroClienteId, "Cliente").Revisar(_db.SolicitudId);
        Assert.IsType<ForbidResult>(r);
    }

    [Theory]
    [InlineData(nameof(RevisionClienteController.Historial), "Administrador")]
    [InlineData(nameof(RevisionClienteController.Notificaciones), "Administrador")]
    [InlineData(nameof(RevisionClienteController.Aprobar), "Cliente")]
    [InlineData(nameof(RevisionClienteController.Rechazar), "Cliente")]
    [InlineData(nameof(RevisionClienteController.SolicitarModificacion), "Cliente")]
    public void Acciones_exigen_el_rol_correcto(string accion, string rol)
    {
        var metodo = typeof(RevisionClienteController).GetMethods().First(m => m.Name == accion);
        var atributo = metodo.GetCustomAttribute<AuthorizeAttribute>();
        Assert.Equal(rol, atributo?.Roles);
        if (rol == "Cliente")
            Assert.NotNull(metodo.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>());
    }

    // ---------------------------------------------------- Regla crítica: pedidos

    [Fact]
    public async Task No_se_crea_pedido_si_la_propuesta_no_esta_aprobada_por_el_cliente()
    {
        using var ctx = _db.NuevoContexto();
        var compras = TestDb.ConUsuario(new ComprasController(ctx, TestDb.UserManager(ctx)), _db.ClienteId, "Cliente");

        var r = await compras.Confirmar(_db.CotizacionId, "SINPE", null);

        var redirect = Assert.IsType<RedirectToActionResult>(r);
        Assert.Equal("VerCotizacion", redirect.ActionName);
        Assert.Empty(ctx.Compras);
    }

    [Fact]
    public async Task Cotizacion_marcada_aprobada_sin_flujo_M15_no_genera_pedido()
    {
        // Un cambio manual de estado (sin congelación ni aprobación del cliente) no basta.
        using (var ctx = _db.NuevoContexto())
        {
            var cot = await ctx.Cotizaciones.SingleAsync();
            cot.Estado = EstadosRevision.CotAprobada;
            await ctx.SaveChangesAsync();
        }

        using var ctx2 = _db.NuevoContexto();
        var compras = TestDb.ConUsuario(new ComprasController(ctx2, TestDb.UserManager(ctx2)), _db.ClienteId, "Cliente");
        await compras.Confirmar(_db.CotizacionId, "SINPE", null);
        Assert.Empty(ctx2.Compras);
    }

    [Fact]
    public async Task Mis_compras_muestra_la_propuesta_aprobada_pendiente_de_confirmar()
    {
        using (var ctx = _db.NuevoContexto())
        {
            var compras = TestDb.ConUsuario(new ComprasController(ctx, TestDb.UserManager(ctx)), _db.ClienteId, "Cliente");
            var antes = Assert.IsType<ViewResult>(await compras.MisCompras());
            Assert.Empty((List<Cotizacion>)antes.ViewData["PendientesConfirmar"]!);
        }

        await AprobarComoClienteAsync();

        using var ctx2 = _db.NuevoContexto();
        var compras2 = TestDb.ConUsuario(new ComprasController(ctx2, TestDb.UserManager(ctx2)), _db.ClienteId, "Cliente");
        var despues = Assert.IsType<ViewResult>(await compras2.MisCompras());
        var pendiente = Assert.Single((List<Cotizacion>)despues.ViewData["PendientesConfirmar"]!);
        Assert.NotNull(pendiente.OrdenPreliminar);

        // Otro cliente no ve la propuesta ajena.
        var otro = TestDb.ConUsuario(new ComprasController(ctx2, TestDb.UserManager(ctx2)), _db.OtroClienteId, "Cliente");
        var deOtro = Assert.IsType<ViewResult>(await otro.MisCompras());
        Assert.Empty((List<Cotizacion>)deOtro.ViewData["PendientesConfirmar"]!);
    }

    [Fact]
    public async Task Pedido_se_crea_despues_de_la_aprobacion_del_cliente()
    {
        await AprobarComoClienteAsync();

        using var ctx = _db.NuevoContexto();
        var compras = TestDb.ConUsuario(new ComprasController(ctx, TestDb.UserManager(ctx)), _db.ClienteId, "Cliente");
        var r = await compras.Confirmar(_db.CotizacionId, "SINPE", null);

        Assert.Equal(nameof(ComprasController.Details), Assert.IsType<RedirectToActionResult>(r).ActionName);
        Assert.Single(ctx.Compras);
    }

    [Fact]
    public async Task Produccion_solo_avanza_con_propuesta_aprobada()
    {
        using (var ctx = _db.NuevoContexto())
        {
            var cotizaciones = TestDb.ConUsuario(
                new CotizacionesController(ctx, TestDb.UserManager(ctx), null!), _db.AdminId, "Administrador");
            await cotizaciones.CambiarEstado(_db.SolicitudId, "En producción");
            Assert.Equal(EstadosRevision.Cotizado, (await ctx.SolicitudesCotizacion.AsNoTracking()
                .SingleAsync(s => s.Id == _db.SolicitudId)).Estado);
        }

        await AprobarComoClienteAsync();

        using (var ctx = _db.NuevoContexto())
        {
            var cotizaciones = TestDb.ConUsuario(
                new CotizacionesController(ctx, TestDb.UserManager(ctx), null!), _db.AdminId, "Administrador");
            await cotizaciones.CambiarEstado(_db.SolicitudId, "En producción");
            Assert.Equal("En producción", (await ctx.SolicitudesCotizacion.AsNoTracking()
                .SingleAsync(s => s.Id == _db.SolicitudId)).Estado);
        }
    }

    [Fact]
    public async Task Cotizacion_congelada_no_se_puede_editar()
    {
        await AprobarComoClienteAsync();

        using var ctx = _db.NuevoContexto();
        var cotizaciones = TestDb.ConUsuario(
            new CotizacionesController(ctx, TestDb.UserManager(ctx), null!), _db.AdminId, "Administrador");

        Assert.IsType<BadRequestObjectResult>(await cotizaciones.EditCotizacion(_db.CotizacionId));
    }
}

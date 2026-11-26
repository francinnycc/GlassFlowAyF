using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using GlassFlowAyF.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers;

/// <summary>Módulo 15 – Revisión y Aprobación del Cliente.</summary>
[Authorize]
public class RevisionClienteController : Controller
{
    private readonly IRevisionClienteService _revision;
    private readonly ApplicationDbContext _context;

    public RevisionClienteController(IRevisionClienteService revision, ApplicationDbContext context)
    {
        _revision = revision;
        _context = context;
    }

    private string? UsuarioId =>
        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    private bool EsAdmin => User.IsInRole("Administrador");

    // =========================================================
    // HU 1 – CONSULTA DE PROPUESTA Y COTIZACIÓN
    // =========================================================

    [Authorize(Roles = "Cliente,Administrador")]
    [HttpGet]
    public async Task<IActionResult> Revisar(int id)
    {
        if (UsuarioId == null) return Challenge();

        var resultado = await _revision.ObtenerRevisionAsync(id, UsuarioId, EsAdmin);
        return resultado.Codigo switch
        {
            CodigoRevision.NoEncontrado => NotFound(),
            CodigoRevision.Prohibido => Forbid(),
            _ => View(resultado.Modelo)
        };
    }

    /// <summary>Contenido del modal de detalles técnicos (escenario 1.3).</summary>
    [Authorize(Roles = "Cliente,Administrador")]
    [HttpGet]
    public async Task<IActionResult> DetallePropuesta(int id)
    {
        if (UsuarioId == null) return Challenge();

        var solicitudId = await _context.Cotizaciones
            .Where(c => c.Id == id)
            .Select(c => (int?)c.SolicitudCotizacionId)
            .FirstOrDefaultAsync();
        if (solicitudId == null) return NotFound();

        var resultado = await _revision.ObtenerRevisionAsync(solicitudId.Value, UsuarioId, EsAdmin, id);
        return resultado.Codigo switch
        {
            CodigoRevision.NoEncontrado => NotFound(),
            CodigoRevision.Prohibido => Forbid(),
            _ => PartialView("_DetallePropuesta", resultado.Modelo)
        };
    }

    // =========================================================
    // HU 2 / HU 3 – DECISIONES DEL CLIENTE
    // =========================================================

    [Authorize(Roles = "Cliente")]
    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> Aprobar(int cotizacionId, Guid version, bool aceptoTerminos) =>
        Procesar(cotizacionId, version, null, aceptoTerminos, _revision.AprobarAsync);

    [Authorize(Roles = "Cliente")]
    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> SolicitarModificacion(int cotizacionId, Guid version, string? comentario) =>
        Procesar(cotizacionId, version, comentario, false, _revision.SolicitarModificacionAsync);

    [Authorize(Roles = "Cliente")]
    [HttpPost, ValidateAntiForgeryToken]
    public Task<IActionResult> Rechazar(int cotizacionId, Guid version, string? comentario) =>
        Procesar(cotizacionId, version, comentario, false, _revision.RechazarAsync);

    private async Task<IActionResult> Procesar(int cotizacionId, Guid version, string? comentario,
        bool aceptoTerminos, Func<ComandoRevision, Task<ResultadoRevision>> accion)
    {
        if (UsuarioId == null) return Challenge();

        var comando = new ComandoRevision(
            cotizacionId,
            version,
            comentario,
            aceptoTerminos,
            UsuarioId,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            Request.Headers.UserAgent.ToString());

        var resultado = await accion(comando);

        switch (resultado.Codigo)
        {
            case CodigoRevision.NoEncontrado:
                return NotFound();
            case CodigoRevision.Prohibido:
                return Forbid();
            case CodigoRevision.Ok:
                TempData["Mensaje"] = resultado.Mensaje;
                break;
            default:
                TempData["Error"] = resultado.Mensaje;
                // Se conserva el texto escrito para no obligar al cliente a redactarlo de nuevo.
                if (!string.IsNullOrEmpty(comentario)) TempData["ComentarioPrevio"] = comentario;
                break;
        }

        return RedirectToAction(nameof(Revisar), new { id = resultado.SolicitudId });
    }

    // =========================================================
    // ADMINISTRACIÓN – BITÁCORA Y NOTIFICACIONES
    // =========================================================

    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public async Task<IActionResult> Historial(int id)
    {
        var solicitud = await _context.SolicitudesCotizacion.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
        if (solicitud == null) return NotFound();

        var revisiones = await _context.RevisionesCliente.AsNoTracking()
            .Include(r => r.Usuario)
            .Where(r => r.SolicitudCotizacionId == id)
            .OrderByDescending(r => r.FechaUtc)
            .ToListAsync();

        ViewBag.Solicitud = solicitud;
        ViewBag.Ordenes = await _context.OrdenesPreliminares.AsNoTracking()
            .Where(o => o.SolicitudCotizacionId == id).ToListAsync();
        return View(revisiones);
    }

    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public async Task<IActionResult> Notificaciones(bool todas = false)
    {
        var consulta = _context.Notificaciones.AsNoTracking();
        if (!todas) consulta = consulta.Where(n => !n.Leida);

        ViewBag.Todas = todas;
        return View(await consulta.OrderByDescending(n => n.Fecha).Take(200).ToListAsync());
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarcarLeida(int id)
    {
        var notificacion = await _context.Notificaciones.FindAsync(id);
        if (notificacion == null) return NotFound();

        notificacion.Leida = true;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Notificaciones));
    }
}

using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using GlassFlowAyF.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers;

[Authorize(Roles = "Administrador,Cliente")]
public class FotografiasController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    private readonly IOpenAIService _ia;
    public FotografiasController(ApplicationDbContext db, UserManager<ApplicationUser> users, IOpenAIService ia)
    { _db = db; _users = users; _ia = ia; }

    [HttpGet]
    public async Task<IActionResult> Historial(int solicitudId)
    {
        var solicitud = await _db.SolicitudesCotizacion.AsNoTracking()
            .Include(s => s.Fotografias).Include(s => s.Producto).Include(s => s.Material)
            .FirstOrDefaultAsync(s => s.Id == solicitudId);
        if (solicitud == null) return NotFound();
        if (User.IsInRole("Cliente"))
        {
            var usuario = await _users.GetUserAsync(User);
            if (usuario == null || !string.Equals(usuario.Email, solicitud.Correo, StringComparison.OrdinalIgnoreCase))
                return Forbid();
        }
        var ids = solicitud.Fotografias.Select(f => f.Id).ToArray();
        ViewBag.Revisiones = await _db.RevisionesFotografias.AsNoTracking()
            .Where(r => ids.Contains(r.SolicitudFotografiaId))
            .OrderByDescending(r => r.FechaRevision).ThenByDescending(r => r.Id).ToListAsync();
        return View(solicitud);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Revisar(int fotografiaId, string estado, string? observaciones)
    {
        var foto = await _db.SolicitudFotografias.FirstOrDefaultAsync(f => f.Id == fotografiaId);
        if (foto == null) return NotFound();
        if (estado != "Aprobada" && estado != "Requiere nueva fotografía" && estado != "Pendiente")
        { TempData["Error"] = "Seleccione un estado válido."; return RedirectToAction(nameof(Historial), new { solicitudId = foto.SolicitudCotizacionId }); }
        if ((observaciones?.Length ?? 0) > 1500 || (estado == "Requiere nueva fotografía" && string.IsNullOrWhiteSpace(observaciones)))
        { TempData["Error"] = "Ingrese observaciones válidas (máximo 1500 caracteres)."; return RedirectToAction(nameof(Historial), new { solicitudId = foto.SolicitudCotizacionId }); }
        _db.RevisionesFotografias.Add(new RevisionFotografia
        {
            SolicitudFotografiaId = foto.Id,
            Estado = estado,
            Observaciones = observaciones?.Trim(),
            Autor = User.Identity?.Name ?? "Administrador"
        });
        await _db.SaveChangesAsync();
        TempData["Mensaje"] = "Revisión registrada en el historial.";
        return RedirectToAction(nameof(Historial), new { solicitudId = foto.SolicitudCotizacionId });
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Analizar(int fotografiaId, CancellationToken cancellationToken)
    {
        var foto = await _db.SolicitudFotografias.Include(f => f.SolicitudCotizacion)
            .FirstOrDefaultAsync(f => f.Id == fotografiaId, cancellationToken);
        if (foto?.SolicitudCotizacion == null) return NotFound();
        try
        {
            // Reutiliza el servicio existente que envía la fotografía al modelo multimodal.
            var texto = await _ia.GenerarRecomendacionAsync(foto.SolicitudCotizacion, foto,
                "Evaluación fotográfica técnica preliminar", "Según fotografía", "Espacio fotografiado",
                "Analiza específicamente la imagen: describe lo observable, calidad y suficiencia de la evidencia, " +
                "posibles obstáculos visibles para instalar vidrio/aluminio, datos faltantes y preguntas para visita técnica. " +
                "No inventes medidas, materiales ocultos ni certificaciones. Distingue observaciones visuales de hipótesis.", cancellationToken);
            _db.RevisionesFotografias.Add(new RevisionFotografia
            {
                SolicitudFotografiaId = foto.Id,
                Estado = "Análisis IA",
                AnalisisIA = texto.Length > 5000 ? texto[..5000] : texto,
                ModeloIA = _ia.ModeloTexto,
                Autor = User.Identity?.Name ?? "Administrador"
            });
            await _db.SaveChangesAsync(cancellationToken);
            TempData["Mensaje"] = "Análisis de IA guardado en el historial. Requiere validación humana.";
        }
        catch (Exception)
        { TempData["Error"] = "No se pudo completar el análisis. Verifique la configuración y conectividad de OpenAI."; }
        return RedirectToAction(nameof(Historial), new { solicitudId = foto.SolicitudCotizacionId });
    }
}

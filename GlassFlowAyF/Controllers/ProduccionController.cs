using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using GlassFlowAyF.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace GlassFlowAyF.Controllers;

[Authorize(Roles = "Administrador")]
public class ProduccionController : Controller
{
    private readonly ApplicationDbContext _db;
    public ProduccionController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? estado)
    {
        var query = _db.OrdenesProduccion.AsNoTracking().Include(x => x.Compra).ThenInclude(x => x.Cotizacion).ThenInclude(x => x!.SolicitudCotizacion).AsQueryable();
        if (!string.IsNullOrWhiteSpace(estado)) query = query.Where(x => x.Estado == estado);
        ViewBag.Estado = estado;
        return View(await query.OrderByDescending(x => x.FechaCreacion).ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? compraId)
    {
        ViewBag.Compras = await ComprasElegibles();
        return View(new ProduccionFormViewModel { CompraId = compraId ?? 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProduccionFormViewModel model)
    {
        if (model.FechaFinPrevista.Date < model.FechaInicioPrevista.Date)
            ModelState.AddModelError(nameof(model.FechaFinPrevista), "La finalización no puede ser anterior al inicio.");
        var compra = await _db.Compras.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.CompraId);
        if (compra == null || compra.FechaPago == null || compra.Estado == "Pendiente de pago")
            ModelState.AddModelError(nameof(model.CompraId), "Seleccioná una compra con pago confirmado.");
        if (string.IsNullOrWhiteSpace(model.Responsable))
            ModelState.AddModelError(nameof(model.Responsable), "Asigná un responsable de producción.");
        if (await _db.OrdenesProduccion.AnyAsync(x => x.CompraId == model.CompraId))
            ModelState.AddModelError(nameof(model.CompraId), "Esta compra ya tiene una orden de producción.");
        if (!ModelState.IsValid) { ViewBag.Compras = await ComprasElegibles(); return View(model); }
        var orden = new OrdenProduccion
        {
            CompraId = model.CompraId,
            NumeroOrden = "TEMP-" + Guid.NewGuid().ToString("N")[..12],
            FechaInicioPrevista = model.FechaInicioPrevista.Date,
            FechaFinPrevista = model.FechaFinPrevista.Date,
            Responsable = model.Responsable?.Trim(),
            Recursos = model.Recursos?.Trim(),
            Observaciones = model.Observaciones?.Trim()
        };
        _db.OrdenesProduccion.Add(orden);
        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateException) { ModelState.AddModelError("", "No se pudo crear la orden. Comprobá que la compra no tenga otra orden."); ViewBag.Compras = await ComprasElegibles(); return View(model); }
        orden.NumeroOrden = $"GF-PROD-{orden.Id:00000}";
        await _db.SaveChangesAsync();
        TempData["Success"] = "Orden de producción creada.";
        return RedirectToAction(nameof(Details), new { id = orden.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var orden = await _db.OrdenesProduccion.AsNoTracking()
            .Include(x => x.Compra).ThenInclude(x => x.Cotizacion).ThenInclude(x => x!.SolicitudCotizacion)
            .Include(x => x.Materiales).ThenInclude(x => x.Material)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (orden == null) return NotFound();
        ViewBag.Materiales = await _db.Materiales.AsNoTracking().Where(x => x.Activo).OrderBy(x => x.Nombre).ToListAsync();
        return View(orden);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado)
    {
        var orden = await _db.OrdenesProduccion.Include(x => x.Materiales).FirstOrDefaultAsync(x => x.Id == id);
        if (orden == null) return NotFound();
        bool permitido = (orden.Estado == "Pendiente" && nuevoEstado == "En proceso") ||
                          (orden.Estado == "En proceso" && (nuevoEstado == "Terminada" || nuevoEstado == "Pausada")) ||
                          (orden.Estado == "Pausada" && nuevoEstado == "En proceso");
        if (!permitido) TempData["Error"] = "Transición de estado no permitida.";
        else if (nuevoEstado == "En proceso" && string.IsNullOrWhiteSpace(orden.Responsable))
            TempData["Error"] = "Asigná un responsable antes de iniciar la producción.";
        else if (nuevoEstado == "Terminada" && string.IsNullOrWhiteSpace(orden.Responsable))
            TempData["Error"] = "Asigná un responsable antes de finalizar la producción.";
        else if (nuevoEstado == "Terminada" && orden.Materiales.Count == 0)
            TempData["Error"] = "Registrá al menos un material antes de finalizar la producción.";
        else if (nuevoEstado == "Terminada" && orden.Materiales.Any(x => !x.ConfirmadoDisponible))
            TempData["Error"] = "Confirmá la disponibilidad de todos los materiales antes de terminar.";
        else
        {
            orden.Estado = nuevoEstado;
            if (nuevoEstado == "En proceso" && orden.FechaInicioReal == null) orden.FechaInicioReal = DateTime.Now;
            if (nuevoEstado == "Terminada") orden.FechaFinReal = DateTime.Now;
            await _db.SaveChangesAsync(); TempData["Success"] = "Estado actualizado.";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarMaterial(ProduccionMaterialViewModel model)
    {
        var orden = await _db.OrdenesProduccion.FindAsync(model.OrdenId);
        if (orden == null) return NotFound();
        if (orden.Estado == "Terminada") return BadRequest("La orden ya está terminada.");
        if (model.Cantidad < 0.01m || model.Cantidad > 9999999999.99m)
            ModelState.AddModelError(nameof(model.Cantidad), "La cantidad debe estar entre 0,01 y 9.999.999.999,99.");
        if (!ModelState.IsValid || !await _db.Materiales.AnyAsync(x => x.Id == model.MaterialId && x.Activo))
        {
            TempData["Error"] = "Verificá el material, la cantidad y la unidad.";
            return RedirectToAction(nameof(Details), new { id = model.OrdenId });
        }
        var existente = await _db.OrdenesProduccionMateriales.FirstOrDefaultAsync(x => x.OrdenProduccionId == model.OrdenId && x.MaterialId == model.MaterialId);
        if (existente != null)
        {
            existente.Cantidad = model.Cantidad; existente.Unidad = model.Unidad.Trim();
            existente.ConfirmadoDisponible = model.ConfirmadoDisponible; existente.Observaciones = model.Observaciones?.Trim();
        }
        else _db.OrdenesProduccionMateriales.Add(new OrdenProduccionMaterial
        {
            OrdenProduccionId = model.OrdenId,
            MaterialId = model.MaterialId,
            Cantidad = model.Cantidad,
            Unidad = model.Unidad.Trim(),
            ConfirmadoDisponible = model.ConfirmadoDisponible,
            Observaciones = model.Observaciones?.Trim()
        });
        await _db.SaveChangesAsync(); TempData["Success"] = "Material registrado.";
        return RedirectToAction(nameof(Details), new { id = model.OrdenId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> QuitarMaterial(int id, int materialRegistroId)
    {
        var orden = await _db.OrdenesProduccion.FindAsync(id);
        if (orden == null) return NotFound();
        if (orden.Estado == "Terminada") return BadRequest("La orden ya está terminada.");
        var registro = await _db.OrdenesProduccionMateriales.FirstOrDefaultAsync(x => x.Id == materialRegistroId && x.OrdenProduccionId == id);
        if (registro == null) return NotFound();
        _db.OrdenesProduccionMateriales.Remove(registro); await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarPlan(int id, DateTime fechaInicioPrevista, DateTime fechaFinPrevista, string? responsable, string? recursos, string? observaciones)
    {
        var orden = await _db.OrdenesProduccion.FindAsync(id);
        if (orden == null) return NotFound();
        if (orden.Estado == "Terminada") return BadRequest("No se puede editar una orden terminada.");
        if (fechaInicioPrevista == default || fechaFinPrevista < fechaInicioPrevista ||
            string.IsNullOrWhiteSpace(responsable) || responsable.Trim().Length > 150 || recursos?.Length > 1000 || observaciones?.Length > 2000)
        {
            TempData["Error"] = "Revisá las fechas y asigná un responsable válido (máximo 150 caracteres).";
            return RedirectToAction(nameof(Details), new { id });
        }
        orden.FechaInicioPrevista = fechaInicioPrevista.Date; orden.FechaFinPrevista = fechaFinPrevista.Date;
        orden.Responsable = responsable?.Trim(); orden.Recursos = recursos?.Trim(); orden.Observaciones = observaciones?.Trim();
        await _db.SaveChangesAsync(); TempData["Success"] = "Planificación actualizada.";
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<List<Compra>> ComprasElegibles() => await _db.Compras.AsNoTracking()
        .Where(x => x.FechaPago != null && x.Estado != "Pendiente de pago" && !_db.OrdenesProduccion.Any(p => p.CompraId == x.Id))
        .OrderByDescending(x => x.FechaCompra).ToListAsync();
}

using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers;

public partial class CotizacionesController
{
    private static bool Editable(Cotizacion c) => c.Compra == null &&
        (c.Estado == "Pendiente" || c.Estado == "Cambios solicitados");

    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public async Task<IActionResult> EditCotizacion(int id)
    {
        var cotizacion = await _context.Cotizaciones.Include(c => c.Compra)
            .Include(c => c.SolicitudCotizacion).ThenInclude(s => s!.Producto)
            .Include(c => c.SolicitudCotizacion).ThenInclude(s => s!.Material)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (cotizacion == null) return NotFound();
        if (!Editable(cotizacion)) return BadRequest("Solo se pueden ajustar cotizaciones pendientes o con cambios solicitados y sin compra.");
        ViewBag.Solicitud = cotizacion.SolicitudCotizacion;
        ViewBag.Editando = true;
        return View("CrearCotizacion", cotizacion);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCotizacion(int id,
        [Bind("Version,CostoMateriales,ManoObra,CostoInstalacion,OtrosCostos,PorcentajeImpuesto,Descuento,FechaVencimiento,DetalleTecnico,Condiciones")] Cotizacion model)
    {
        var actual = await _context.Cotizaciones.Include(c => c.Compra)
            .Include(c => c.SolicitudCotizacion).FirstOrDefaultAsync(c => c.Id == id);
        if (actual == null) return NotFound();
        if (!Editable(actual)) return BadRequest("Esta cotización ya no permite ajustes.");
        if (model.Version != actual.Version) return Conflict("La cotización cambió. Recargue antes de editarla.");
        ValidarImportes(model);
        if (model.FechaVencimiento.Date < DateTime.Today)
            ModelState.AddModelError(nameof(model.FechaVencimiento), "La fecha de vencimiento no puede ser anterior a hoy.");
        if (!ModelState.IsValid)
        {
            model.Id = actual.Id;
            model.SolicitudCotizacionId = actual.SolicitudCotizacionId;
            ViewBag.Solicitud = actual.SolicitudCotizacion;
            ViewBag.Editando = true;
            return View("CrearCotizacion", model);
        }
        actual.CostoMateriales = model.CostoMateriales;
        actual.ManoObra = model.ManoObra;
        actual.CostoInstalacion = model.CostoInstalacion;
        actual.OtrosCostos = model.OtrosCostos;
        actual.PorcentajeImpuesto = model.PorcentajeImpuesto;
        actual.Descuento = model.Descuento;
        actual.FechaVencimiento = model.FechaVencimiento;
        actual.DetalleTecnico = model.DetalleTecnico;
        actual.Condiciones = model.Condiciones;
        CalcularCotizacion(actual);
        actual.Estado = "Pendiente";
        actual.FechaRespuestaCliente = null;
        actual.Version = Guid.NewGuid();
        actual.SolicitudCotizacion!.MontoEstimado = actual.Total;
        actual.SolicitudCotizacion.Estado = "Cotizado";
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException) { return Conflict("La cotización cambió. Recargue antes de editarla."); }
        TempData["Mensaje"] = "Cotización ajustada y disponible para una nueva respuesta del cliente.";
        return RedirectToAction(nameof(VerCotizacion), new { id });
    }

    private void ValidarImportes(Cotizacion c)
    {
        foreach (var campo in new[] { nameof(c.CostoMateriales), nameof(c.ManoObra), nameof(c.CostoInstalacion),
            nameof(c.OtrosCostos), nameof(c.Descuento), nameof(c.PorcentajeImpuesto) })
        {
            var valor = (decimal)typeof(Cotizacion).GetProperty(campo)!.GetValue(c)!;
            if (decimal.Round(valor, 2) != valor)
                ModelState.AddModelError(campo, "Use como máximo dos decimales.");
        }
        CalcularCotizacion(c);
        if (c.Descuento > c.Subtotal + c.Impuesto)
            ModelState.AddModelError(nameof(c.Descuento), "El descuento no puede superar el subtotal más el impuesto.");
    }
}

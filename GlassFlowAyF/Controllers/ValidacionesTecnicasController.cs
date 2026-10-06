using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers;

[Authorize(Roles = "Administrador,Cliente")]
public class ValidacionesTecnicasController(ApplicationDbContext context, UserManager<ApplicationUser> users) : Controller
{
    [Authorize(Roles = "Administrador")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Registrar(int solicitudId, RegistrarValidacion model)
    {
        var solicitud = await context.SolicitudesCotizacion.Include(s => s.Cotizaciones)
            .FirstOrDefaultAsync(s => s.Id == solicitudId);
        if (solicitud == null) return NotFound();
        if (solicitud.Estado == "Borrador" || solicitud.Cotizaciones.Any(c => c.Estado == "Aprobada"))
            return BadRequest("No se puede validar un borrador ni una solicitud con cotización aprobada.");
        if (model.Resultado == "Pendiente de información" && string.IsNullOrWhiteSpace(model.InformacionSolicitada))
            ModelState.AddModelError(nameof(model.InformacionSolicitada), "Indique qué información necesita del cliente.");
        if (model.Resultado == "Validada" && (!solicitud.ProductoId.HasValue || !solicitud.MaterialId.HasValue ||
            !(solicitud.Ancho > 0) || !(solicitud.Alto > 0) || solicitud.Cantidad < 1 ||
            (solicitud.RequiereInstalacion && string.IsNullOrWhiteSpace(solicitud.Direccion))))
            ModelState.AddModelError("", "Complete producto, material, medidas, cantidad y dirección antes de validar.");
        if (await context.ValidacionesTecnicas.AnyAsync(v => v.SolicitudCotizacionId == solicitudId &&
            v.Resultado == "Pendiente de información" && v.FechaRespuesta == null))
            ModelState.AddModelError("", "Hay una solicitud de información pendiente de respuesta.");
        if (!ModelState.IsValid)
        {
            TempData["Error"] = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToAction("Details", "Cotizaciones", new { id = solicitudId });
        }
        context.ValidacionesTecnicas.Add(new ValidacionTecnica
        {
            SolicitudCotizacionId = solicitudId, Autor = User.Identity?.Name ?? "Administrador",
            Resultado = model.Resultado, Complejidad = model.Complejidad,
            Observaciones = model.Observaciones.Trim(), RecomiendaVisita = model.RecomiendaVisita,
            InformacionSolicitada = model.Resultado == "Pendiente de información" ? model.InformacionSolicitada!.Trim() : null
        });
        solicitud.Estado = model.Resultado == "Pendiente de información" ? model.Resultado : "En revisión";
        await context.SaveChangesAsync();
        TempData["Mensaje"] = "Validación técnica registrada en el historial.";
        return RedirectToAction("Details", "Cotizaciones", new { id = solicitudId });
    }

    [Authorize(Roles = "Cliente")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Responder(int id, string respuesta)
    {
        var validacion = await context.ValidacionesTecnicas.Include(v => v.SolicitudCotizacion).FirstOrDefaultAsync(v => v.Id == id);
        if (validacion == null) return NotFound();
        var usuario = await users.GetUserAsync(User);
        if (usuario == null || validacion.SolicitudCotizacion?.Correo != usuario.Email) return Forbid();
        if (validacion.Resultado != "Pendiente de información" || validacion.FechaRespuesta != null) return BadRequest();
        if (string.IsNullOrWhiteSpace(respuesta) || respuesta.Length > 2000)
            TempData["Error"] = "Ingrese una respuesta de hasta 2000 caracteres.";
        else
        {
            validacion.RespuestaCliente = respuesta.Trim();
            validacion.FechaRespuesta = DateTime.Now;
            validacion.Version = Guid.NewGuid();
            validacion.SolicitudCotizacion!.Estado = "En revisión";
            try { await context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException) { return Conflict("La respuesta ya fue registrada. Recargue la solicitud."); }
            TempData["Mensaje"] = "Respuesta enviada para revisión técnica.";
        }
        return RedirectToAction("Details", "Cotizaciones", new { id = validacion.SolicitudCotizacionId });
    }
}

using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class VisitasTecnicasController
        : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitasTecnicasController(
            ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var visitas =
                await _context.VisitasTecnicas
                    .Include(v =>
                        v.SolicitudCotizacion)
                    .OrderBy(v => v.FechaHora)
                    .ToListAsync();

            return View(visitas);
        }


        [HttpGet]
        public async Task<IActionResult> Create(
            int? solicitudId)
        {
            await CargarSolicitudes(
                solicitudId);

            return View(
                new VisitaTecnica
                {
                    SolicitudCotizacionId =
                        solicitudId ?? 0,

                    FechaHora =
                        DateTime.Now.AddDays(1)
                });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            VisitaTecnica visita)
        {
            if (visita.FechaHora <=
                DateTime.Now)
            {
                ModelState.AddModelError(
                    nameof(visita.FechaHora),
                    "La visita debe programarse para una fecha futura.");
            }

            var conflicto =
                await _context.VisitasTecnicas
                    .AnyAsync(v =>
                        v.TecnicoAsignado ==
                            visita.TecnicoAsignado &&
                        v.FechaHora ==
                            visita.FechaHora &&
                        v.Estado != "Cancelada");

            if (conflicto)
            {
                ModelState.AddModelError(
                    nameof(visita.FechaHora),
                    "El técnico ya tiene una visita en ese horario.");
            }

            if (!ModelState.IsValid)
            {
                await CargarSolicitudes(
                    visita.SolicitudCotizacionId);

                return View(visita);
            }

            _context.VisitasTecnicas
                .Add(visita);

            var solicitud =
                await _context
                    .SolicitudesCotizacion
                    .FindAsync(
                        visita
                            .SolicitudCotizacionId);

            if (solicitud != null)
            {
                solicitud.Estado =
                    "Visita programada";
            }

            await _context
                .SaveChangesAsync();

            TempData["Mensaje"] =
                "Visita técnica programada correctamente.";

            return RedirectToAction(
                nameof(Index));
        }


        private async Task CargarSolicitudes(
            int? seleccionada)
        {
            var solicitudes =
                await _context
                    .SolicitudesCotizacion
                    .OrderByDescending(
                        s => s.FechaSolicitud)
                    .Select(s => new
                    {
                        s.Id,

                        Texto =
                            $"#SOL-{s.Id:D5} - " +
                            $"{s.NombreCliente} - " +
                            $"{s.TipoProducto}"
                    })
                    .ToListAsync();

            ViewBag.Solicitudes =
                new SelectList(
                    solicitudes,
                    "Id",
                    "Texto",
                    seleccionada);
        }
    }
}
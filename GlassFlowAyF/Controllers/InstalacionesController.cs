using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class InstalacionesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InstalacionesController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var trabajos =
                await _context.TrabajosInstalacion
                    .AsNoTracking()
                    .OrderBy(
                        t => t.FechaInstalacion)
                    .ToListAsync();

            return View(trabajos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(
                new TrabajoInstalacion
                {
                    FechaInstalacion =
                        DateTime.Now.AddDays(2)
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            TrabajoInstalacion trabajo)
        {
            if (!ModelState.IsValid)
            {
                return View(trabajo);
            }

            trabajo.Estado = "Programada";

            _context.TrabajosInstalacion
                .Add(trabajo);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Trabajo de instalación creado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Checklist(
            int id)
        {
            var trabajo =
                await _context.TrabajosInstalacion
                    .FindAsync(id);

            if (trabajo == null)
            {
                return NotFound();
            }

            return View(trabajo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checklist(
            TrabajoInstalacion form)
        {
            var trabajo =
                await _context.TrabajosInstalacion
                    .FindAsync(form.Id);

            if (trabajo == null)
            {
                return NotFound();
            }

            trabajo.MedidasConfirmadas =
                form.MedidasConfirmadas;

            trabajo.MaterialListo =
                form.MaterialListo;

            trabajo.InstalacionRealizada =
                form.InstalacionRealizada;

            trabajo.RevisionAcabados =
                form.RevisionAcabados;

            trabajo.LimpiezaFinal =
                form.LimpiezaFinal;

            trabajo.Observaciones =
                form.Observaciones;

            if (
                trabajo.MedidasConfirmadas &&
                trabajo.MaterialListo &&
                trabajo.InstalacionRealizada &&
                trabajo.RevisionAcabados &&
                trabajo.LimpiezaFinal)
            {
                trabajo.Estado = "Finalizada";
            }
            else
            {
                trabajo.Estado = "En proceso";
            }

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Checklist actualizado correctamente.";

            return RedirectToAction(
                nameof(Checklist),
                new { id = trabajo.Id });
        }
    }
}
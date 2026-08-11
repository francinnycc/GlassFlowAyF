using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers
{
    [Authorize(Roles = "Instalador")]
    public class InstaladorController
        : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser>
            _userManager;


        public InstaladorController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> MiPanel()
        {
            var usuario =
                await _userManager.GetUserAsync(
                    User);

            if (usuario == null)
            {
                return Unauthorized();
            }


            var trabajos =
                await _context.TrabajosInstalacion
                    .AsNoTracking()
                    .Where(t =>
                        t.InstaladorId ==
                        usuario.Id)
                    .OrderBy(t =>
                        t.FechaInstalacion)
                    .ToListAsync();


            ViewBag.Pendientes =
                trabajos.Count(t =>
                    t.Estado ==
                    "Programada");

            ViewBag.EnProceso =
                trabajos.Count(t =>
                    t.Estado ==
                    "En proceso");

            ViewBag.Finalizados =
                trabajos.Count(t =>
                    t.Estado ==
                    "Finalizada");


            return View(trabajos);
        }


        public async Task<IActionResult> Details(
            int id)
        {
            var usuario =
                await _userManager.GetUserAsync(
                    User);


            var trabajo =
                await _context.TrabajosInstalacion
                    .Include(t =>
                        t.SolicitudCotizacion)
                        .ThenInclude(s =>
                            s!.Producto)
                    .Include(t =>
                        t.SolicitudCotizacion)
                        .ThenInclude(s =>
                            s!.Material)
                    .Include(t =>
                        t.SolicitudCotizacion)
                        .ThenInclude(s =>
                            s!.Fotografias)
                    .FirstOrDefaultAsync(
                        t =>
                            t.Id == id &&
                            t.InstaladorId ==
                            usuario!.Id);


            if (trabajo == null)
            {
                return NotFound();
            }


            return View(trabajo);
        }


        [HttpGet]
        public async Task<IActionResult> Checklist(
            int id)
        {
            var usuario =
                await _userManager.GetUserAsync(
                    User);


            var trabajo =
                await _context.TrabajosInstalacion
                    .FirstOrDefaultAsync(
                        t =>
                            t.Id == id &&
                            t.InstaladorId ==
                            usuario!.Id);


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
            var usuario =
                await _userManager.GetUserAsync(
                    User);


            var trabajo =
                await _context.TrabajosInstalacion
                    .Include(t =>
                        t.SolicitudCotizacion)
                    .Include(t =>
                        t.Compra)
                    .FirstOrDefaultAsync(
                        t =>
                            t.Id == form.Id &&
                            t.InstaladorId ==
                            usuario!.Id);


            if (trabajo == null)
            {
                return NotFound();
            }


            trabajo.MedidasConfirmadas =
                form.MedidasConfirmadas;

            trabajo.MaterialListo =
                form.MaterialListo;

            trabajo.InstalacionIniciada =
                form.InstalacionIniciada;

            trabajo.InstalacionRealizada =
                form.InstalacionRealizada;

            trabajo.RevisionAcabados =
                form.RevisionAcabados;

            trabajo.LimpiezaFinal =
                form.LimpiezaFinal;

            trabajo.Observaciones =
                form.Observaciones;


            if (trabajo.InstalacionIniciada &&
                trabajo.Estado ==
                "Programada")
            {
                trabajo.Estado =
                    "En proceso";
            }


            if (
                trabajo.MedidasConfirmadas &&
                trabajo.MaterialListo &&
                trabajo.InstalacionIniciada &&
                trabajo.InstalacionRealizada &&
                trabajo.RevisionAcabados &&
                trabajo.LimpiezaFinal)
            {
                trabajo.Estado =
                    "Finalizada";

                trabajo.FechaFinalizacion =
                    DateTime.Now;


                if (trabajo
                    .SolicitudCotizacion != null)
                {
                    trabajo
                        .SolicitudCotizacion
                        .Estado =
                            "Finalizado";
                }


                if (trabajo.Compra != null)
                {
                    trabajo.Compra.Estado =
                        "Finalizada";
                }
            }


            await _context
                .SaveChangesAsync();


            TempData["Mensaje"] =
                "Avance actualizado correctamente.";


            return RedirectToAction(
                nameof(Checklist),
                new { id = trabajo.Id });
        }
    }
}
using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class InstalacionesController
        : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser>
            _userManager;


        public InstalacionesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var trabajos =
                await _context.TrabajosInstalacion
                    .Include(t => t.Instalador)
                    .Include(t => t.Compra)
                    .OrderBy(t =>
                        t.FechaInstalacion)
                    .ToListAsync();

            return View(trabajos);
        }


        [HttpGet]
        public async Task<IActionResult> Create(
            int? compraId)
        {
            await CargarCombos(
                compraId,
                null);

            return View(
                new TrabajoInstalacion
                {
                    CompraId =
                        compraId,

                    FechaInstalacion =
                        DateTime.Now.AddDays(3)
                });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            TrabajoInstalacion trabajo)
        {
            if (!trabajo.CompraId.HasValue)
            {
                ModelState.AddModelError(
                    nameof(trabajo.CompraId),
                    "Seleccione una compra.");
            }

            if (string.IsNullOrWhiteSpace(
                trabajo.InstaladorId))
            {
                ModelState.AddModelError(
                    nameof(trabajo.InstaladorId),
                    "Seleccione un instalador.");
            }


            var compra =
                trabajo.CompraId.HasValue
                    ? await _context.Compras
                        .Include(c => c.Cliente)
                        .Include(c => c.Cotizacion)
                            .ThenInclude(c =>
                                c!.SolicitudCotizacion)
                                .ThenInclude(s =>
                                    s!.Producto)
                        .FirstOrDefaultAsync(
                            c =>
                                c.Id ==
                                trabajo.CompraId)
                    : null;


            if (compra == null)
            {
                ModelState.AddModelError(
                    nameof(trabajo.CompraId),
                    "La compra seleccionada no existe.");
            }
            else if (compra.Estado !=
                "En producción")
            {
                ModelState.AddModelError(
                    nameof(trabajo.CompraId),
                    "La compra debe tener el pago confirmado.");
            }


            if (!ModelState.IsValid)
            {
                await CargarCombos(
                    trabajo.CompraId,
                    trabajo.InstaladorId);

                return View(trabajo);
            }


            var solicitud =
                compra!.Cotizacion?
                    .SolicitudCotizacion;


            trabajo.Cliente =
                compra.Cliente?
                    .NombreCompleto
                ??
                solicitud?.NombreCliente
                ??
                "";

            trabajo.Producto =
                solicitud?.Producto?.Nombre
                ??
                solicitud?.TipoProducto
                ??
                "";

            trabajo.Direccion =
                solicitud?.Direccion ?? "";

            trabajo.SolicitudCotizacionId =
                solicitud?.Id;

            trabajo.Estado =
                "Programada";


            _context.TrabajosInstalacion
                .Add(trabajo);


            solicitud!.Estado =
                "Instalación programada";


            await _context
                .SaveChangesAsync();


            TempData["Mensaje"] =
                "Instalación asignada correctamente.";


            return RedirectToAction(
                nameof(Index));
        }


        private async Task CargarCombos(
            int? compraId,
            string? instaladorId)
        {
            var compras =
                await _context.Compras
                    .Include(c => c.Cliente)
                    .Include(c => c.Cotizacion)
                        .ThenInclude(c =>
                            c!.SolicitudCotizacion)
                    .Where(c =>
                        c.Estado ==
                            "En producción" &&
                        c.TrabajoInstalacion ==
                            null)
                    .Select(c => new
                    {
                        c.Id,

                        Texto =
                            $"{c.NumeroOrden} - " +
                            $"{c.Cliente!.NombreCompleto} - " +
                            $"{c.Cotizacion!.SolicitudCotizacion!.TipoProducto}"
                    })
                    .ToListAsync();


            ViewBag.Compras =
                new SelectList(
                    compras,
                    "Id",
                    "Texto",
                    compraId);


            var instaladores =
                await _userManager
                    .GetUsersInRoleAsync(
                        "Instalador");


            ViewBag.Instaladores =
                new SelectList(
                    instaladores
                        .Where(u => u.Activo),
                    "Id",
                    "NombreCompleto",
                    instaladorId);
        }
    }
}
using System.Diagnostics;
using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            if (User.IsInRole("Administrador"))
            {
                return RedirectToAction(
                    nameof(Admin));
            }

            if (User.IsInRole("Instalador"))
            {
                return RedirectToAction(
                    "MiPanel",
                    "Instalador");
            }

            if (User.IsInRole("Cliente"))
            {
                return RedirectToAction(
                    nameof(Cliente));
            }

            return RedirectToAction(
                nameof(Index));
        }


        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Admin()
        {
            ViewBag.TotalProductos =
                await _context.Productos
                    .CountAsync(p => p.Activo);

            ViewBag.TotalSolicitudes =
                await _context
                    .SolicitudesCotizacion
                    .CountAsync();

            ViewBag.SolicitudesPendientes =
                await _context
                    .SolicitudesCotizacion
                    .CountAsync(s =>
                        s.Estado == "Solicitado" ||
                        s.Estado == "En revisión");

            ViewBag.Visitas =
                await _context.VisitasTecnicas
                    .CountAsync(v =>
                        v.Estado == "Programada");

            ViewBag.Instalaciones =
                await _context
                    .TrabajosInstalacion
                    .CountAsync(t =>
                        t.Estado != "Finalizada");

            ViewBag.ComprasPendientes =
                await _context.Compras
                    .CountAsync(c =>
                        c.Estado ==
                        "Pendiente de pago");

            ViewBag.SolicitudesRecientes =
                await _context
                    .SolicitudesCotizacion
                    .Include(s => s.Producto)
                    .OrderByDescending(
                        s => s.FechaSolicitud)
                    .Take(5)
                    .ToListAsync();

            return View();
        }


        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Cliente()
        {
            ViewBag.Productos =
                await _context.Productos
                    .Where(p => p.Activo)
                    .OrderBy(p => p.Nombre)
                    .Take(4)
                    .ToListAsync();

            return View();
        }


        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId =
                        Activity.Current?.Id ??
                        HttpContext.TraceIdentifier
                });
        }
    }
}
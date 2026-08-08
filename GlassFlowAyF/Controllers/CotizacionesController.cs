using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers
{
    [Authorize]
    public class CotizacionesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CotizacionesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ADMINISTRADOR: ve todas las solicitudes
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            var solicitudes = await _context.SolicitudesCotizacion
                .AsNoTracking()
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();

            return View(solicitudes);
        }

        // CLIENTE: ve solo sus solicitudes
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> MisSolicitudes()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var solicitudes = await _context.SolicitudesCotizacion
                .AsNoTracking()
                .Where(s => s.Correo == usuario.Email)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();

            return View(solicitudes);
        }

        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var usuario = await _userManager.GetUserAsync(User);

            var model = new SolicitudCotizacion();

            if (usuario != null)
            {
                model.NombreCliente = usuario.NombreCompleto;
                model.Correo = usuario.Email ?? string.Empty;
                model.Telefono = usuario.PhoneNumber ?? string.Empty;
                model.Direccion = usuario.Direccion;
            }

            return View(model);
        }

        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SolicitudCotizacion solicitud)
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return RedirectToAction("Login", "Account");
            }

            solicitud.NombreCliente = usuario.NombreCompleto;
            solicitud.Correo = usuario.Email ?? string.Empty;
            solicitud.Telefono = usuario.PhoneNumber ?? string.Empty;

            if (solicitud.RequiereInstalacion &&
                string.IsNullOrWhiteSpace(solicitud.Direccion))
            {
                ModelState.AddModelError(
                    nameof(solicitud.Direccion),
                    "La dirección es obligatoria cuando solicita instalación.");
            }

            ModelState.Remove(nameof(solicitud.NombreCliente));
            ModelState.Remove(nameof(solicitud.Correo));
            ModelState.Remove(nameof(solicitud.Telefono));

            if (!ModelState.IsValid)
            {
                return View(solicitud);
            }

            solicitud.FechaSolicitud = DateTime.Now;
            solicitud.Estado = "Solicitado";

            _context.SolicitudesCotizacion.Add(solicitud);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                $"Solicitud #{solicitud.Id} registrada correctamente.";

            return RedirectToAction(
                nameof(Details),
                new { id = solicitud.Id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var solicitud = await _context.SolicitudesCotizacion
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (solicitud == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Cliente"))
            {
                var usuario = await _userManager.GetUserAsync(User);

                if (usuario == null ||
                    solicitud.Correo != usuario.Email)
                {
                    return Forbid();
                }
            }

            return View(solicitud);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(
            int id,
            string estado)
        {
            var solicitud =
                await _context.SolicitudesCotizacion.FindAsync(id);

            if (solicitud == null)
            {
                return NotFound();
            }

            string[] estadosPermitidos =
            {
                "Solicitado",
                "En revisión",
                "Pendiente de información",
                "Visita programada",
                "Cotizado",
                "Aprobado",
                "En producción",
                "Instalación programada",
                "Instalado",
                "Finalizado"
            };

            if (!estadosPermitidos.Contains(estado))
            {
                TempData["Error"] =
                    "El estado seleccionado no es válido.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }

            solicitud.Estado = estado;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Estado actualizado correctamente.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }
    }
}

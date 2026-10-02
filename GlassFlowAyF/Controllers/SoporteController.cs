using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using GlassFlowAyF.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers
{
    [Authorize]
    public class SoporteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public SoporteController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // =====================================================
        // CREAR CONSULTA O RECLAMO
        // =====================================================

        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public IActionResult Create(
            string tipo = "Consulta")
        {
            var model =
                new CrearSoporteViewModel
                {
                    Tipo =
                        tipo == "Reclamo"
                            ? "Reclamo"
                            : "Consulta"
                };


            return View(model);
        }


        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CrearSoporteViewModel model)
        {
            if (model.Tipo != "Consulta" &&
                model.Tipo != "Reclamo")
            {
                ModelState.AddModelError(
                    nameof(model.Tipo),
                    "El tipo de solicitud no es válido.");
            }


            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var usuario =
                await _userManager.GetUserAsync(User);


            if (usuario == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }


            var solicitud =
                new SoporteSolicitud
                {
                    UsuarioId = usuario.Id,

                    Tipo = model.Tipo,

                    Categoria =
                        model.Categoria.Trim(),

                    Asunto =
                        model.Asunto.Trim(),

                    Mensaje =
                        model.Mensaje.Trim(),

                    Estado = "Pendiente",

                    FechaCreacion =
                        DateTime.Now
                };


            _context.Set<SoporteSolicitud>()
                .Add(solicitud);


            await _context.SaveChangesAsync();


            TempData["Mensaje"] =
                model.Tipo == "Reclamo"
                    ? "Tu reclamo fue registrado correctamente."
                    : "Tu consulta fue enviada correctamente.";


            return RedirectToAction(
                nameof(MisSolicitudes));
        }


        // =====================================================
        // SOLICITUDES DEL CLIENTE
        // =====================================================

        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> MisSolicitudes()
        {
            var usuario =
                await _userManager.GetUserAsync(User);


            if (usuario == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }


            var solicitudes =
                await _context
                    .Set<SoporteSolicitud>()

                    .Where(
                        s =>
                            s.UsuarioId ==
                            usuario.Id)

                    .OrderByDescending(
                        s =>
                            s.FechaCreacion)

                    .ToListAsync();


            return View(solicitudes);
        }


        // =====================================================
        // DETALLE
        // =====================================================

        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> Details(
            int id)
        {
            var usuario =
                await _userManager.GetUserAsync(User);


            if (usuario == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }


            var solicitud =
                await _context
                    .Set<SoporteSolicitud>()

                    .FirstOrDefaultAsync(
                        s =>
                            s.Id == id &&
                            s.UsuarioId ==
                            usuario.Id);


            if (solicitud == null)
            {
                return NotFound();
            }


            return View(solicitud);
        }


        // =====================================================
        // CALIFICAR ATENCIÓN
        // =====================================================

        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calificar(
            int id,
            int calificacion,
            string? comentario)
        {
            var usuario =
                await _userManager.GetUserAsync(User);


            if (usuario == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }


            var solicitud =
                await _context
                    .Set<SoporteSolicitud>()

                    .FirstOrDefaultAsync(
                        s =>
                            s.Id == id &&
                            s.UsuarioId ==
                            usuario.Id);


            if (solicitud == null)
            {
                return NotFound();
            }


            if (solicitud.Estado != "Respondido" &&
                solicitud.Estado != "Cerrado")
            {
                TempData["Error"] =
                    "La atención solo puede calificarse después de recibir una respuesta.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }


            if (solicitud.Calificacion.HasValue)
            {
                TempData["Error"] =
                    "Esta atención ya fue calificada.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }


            if (calificacion < 1 ||
                calificacion > 5)
            {
                TempData["Error"] =
                    "La calificación debe estar entre 1 y 5.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }


            solicitud.Calificacion =
                calificacion;


            solicitud.ComentarioCalificacion =
                string.IsNullOrWhiteSpace(
                    comentario)
                    ? null
                    : comentario.Trim();


            solicitud.FechaCalificacion =
                DateTime.Now;


            solicitud.Estado =
                "Cerrado";


            await _context.SaveChangesAsync();


            TempData["Mensaje"] =
                "Gracias por calificar la atención recibida.";


            return RedirectToAction(
                nameof(Details),
                new { id });
        }


        // =====================================================
        // ADMINISTRACIÓN
        // =====================================================

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Administrar(
            string? estado = null)
        {
            var query =
                _context
                    .Set<SoporteSolicitud>()

                    .Include(
                        s =>
                            s.Usuario)

                    .AsQueryable();


            if (!string.IsNullOrWhiteSpace(
                    estado))
            {
                query =
                    query.Where(
                        s =>
                            s.Estado ==
                            estado);
            }


            var solicitudes =
                await query

                    .OrderByDescending(
                        s =>
                            s.FechaCreacion)

                    .ToListAsync();


            ViewBag.Estado =
                estado;


            return View(solicitudes);
        }


        // =====================================================
        // RESPONDER
        // =====================================================

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Responder(
            int id)
        {
            var solicitud =
                await _context
                    .Set<SoporteSolicitud>()

                    .Include(
                        s =>
                            s.Usuario)

                    .FirstOrDefaultAsync(
                        s =>
                            s.Id == id);


            if (solicitud == null)
            {
                return NotFound();
            }


            var model =
                new ResponderSoporteViewModel
                {
                    Id =
                        solicitud.Id,

                    Cliente =
                        solicitud.Usuario?
                            .NombreCompleto
                        ?? "Cliente",

                    Email =
                        solicitud.Usuario?
                            .Email
                        ?? string.Empty,

                    Tipo =
                        solicitud.Tipo,

                    Categoria =
                        solicitud.Categoria,

                    Asunto =
                        solicitud.Asunto,

                    Mensaje =
                        solicitud.Mensaje,

                    Respuesta =
                        solicitud.Respuesta
                        ?? string.Empty,

                    Estado =
                        solicitud.Estado ==
                        "Pendiente"
                            ? "Respondido"
                            : solicitud.Estado
                };


            return View(model);
        }


        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Responder(
            ResponderSoporteViewModel model)
        {
            if (model.Estado != "En revisión" &&
                model.Estado != "Respondido" &&
                model.Estado != "Cerrado")
            {
                ModelState.AddModelError(
                    nameof(model.Estado),
                    "Seleccione un estado válido.");
            }


            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var solicitud =
                await _context
                    .Set<SoporteSolicitud>()

                    .Include(
                        s =>
                            s.Usuario)

                    .FirstOrDefaultAsync(
                        s =>
                            s.Id == model.Id);


            if (solicitud == null)
            {
                return NotFound();
            }


            var administrador =
                await _userManager.GetUserAsync(User);


            solicitud.Respuesta =
                model.Respuesta.Trim();


            solicitud.Estado =
                model.Estado;


            solicitud.FechaRespuesta =
                DateTime.Now;


            solicitud.RespondidoPorId =
                administrador?.Id;


            await _context.SaveChangesAsync();


            TempData["Mensaje"] =
                "La solicitud fue respondida correctamente.";


            return RedirectToAction(
                nameof(Administrar));
        }
    }
}
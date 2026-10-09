using DocumentFormat.OpenXml.InkML;
using GlassFlowAyF.Data;
using GlassFlowAyF.Extensions;
using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;


namespace GlassFlowAyF.Controllers
{
    [Authorize]
    [Authorize(Roles = "Administrador,Tecnico")]
    public class VisitasTecnicasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VisitasTecnicasController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index(
    int pagina = 1)
        {
            const int registrosPorPagina = 2;

            var consulta = _context.VisitasTecnicas
                .Include(v => v.SolicitudCotizacion)
                .AsQueryable();

            // El administrador puede ver todas las visitas.
            // El instalador solamente ve las visitas asignadas a él.
            if (!User.IsInRole("Administrador"))
            {
                var usuario = await _userManager.GetUserAsync(User);
                var nombreTecnico = usuario?.NombreCompleto;

                if (string.IsNullOrWhiteSpace(nombreTecnico))
                {
                    return Forbid();
                }

                consulta = consulta.Where(v =>
                    v.TecnicoAsignado == nombreTecnico);
            }

            consulta = consulta
                .OrderBy(v => v.FechaHora);

            var resultado =
                await consulta.ToPagedResultAsync(
                    pagina,
                    registrosPorPagina);

            return View(resultado);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(VisitaTecnica visita)
        {
            if (visita.FechaHora <=
                DateTime.Now)
            {
                ModelState.AddModelError(
                    nameof(visita.FechaHora),
                    "La visita debe programarse para una fecha futura.");
            }

            if (!User.IsInRole("Administrador"))
            {
                var usuario = await _userManager.GetUserAsync(User);

                if (usuario == null ||
                    !string.Equals(
                        visita.TecnicoAsignado,
                        usuario.NombreCompleto,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }
            }

            if (string.IsNullOrWhiteSpace(visita.TecnicoAsignado))
            {
                ModelState.AddModelError(
                    nameof(visita.TecnicoAsignado),
                    "Debe seleccionar un técnico.");
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
                await CargarSolicitudes(visita.SolicitudCotizacionId);
                await CargarTecnicos();

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

        // =========================================================
        // REGISTRAR RESULTADO DE VISITA
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Resultado(int id)
        {
            var visita = await _context.VisitasTecnicas
                .Include(v => v.SolicitudCotizacion)
                .Include(v => v.Fotografias)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (visita == null)
            {
                return NotFound();
            }

            // Solo el administrador o el técnico asignado
            // pueden registrar el resultado.
            if (!User.IsInRole("Administrador"))
            {
                var usuario = await _userManager.GetUserAsync(User);

                if (usuario == null ||
                    !User.IsInRole("Tecnico") ||
                    string.IsNullOrWhiteSpace(usuario.NombreCompleto) ||
                    !string.Equals(
                        visita.TecnicoAsignado,
                        usuario.NombreCompleto,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }
            }

            return View(visita);
        }

        // =========================================================
        // GUARDAR RESULTADO DE VISITA
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resultado(
            int id,
            decimal? ancho,
            decimal? alto,
            decimal? profundidad,
            int cantidad,
            string? resultadoVisita,
            string? observaciones,
            List<IFormFile>? fotos)
        {
            var visita =
                await _context.VisitasTecnicas
                    .Include(v => v.SolicitudCotizacion)
                    .FirstOrDefaultAsync(v =>
                        v.Id == id);

            if (visita == null)
            {
                return NotFound();
            }

            if (!ancho.HasValue ||
                !alto.HasValue)
            {
                ModelState.AddModelError(
                    "",
                    "Debe ingresar el ancho y alto definitivos.");
            }

            if (cantidad <= 0)
            {
                ModelState.AddModelError(
                    "",
                    "La cantidad debe ser mayor que cero.");
            }

            if (fotos != null && fotos.Count > 6)
            {
                ModelState.AddModelError("", "Puede cargar un máximo de 6 fotografías.");
            }

            if (fotos != null)
            {
                foreach (var foto in fotos)
                {
                    if (foto == null || foto.Length == 0)
                        continue;

                    var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();

                    var extensionesPermitidas = new[]
                    {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

                    if (!extensionesPermitidas.Contains(extension))
                    {
                        ModelState.AddModelError(
                            "",
                            $"El archivo {foto.FileName} no tiene un formato permitido.");
                    }

                    if (foto.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError(
                            "",
                            $"El archivo {foto.FileName} supera el tamaño máximo de 5 MB.");
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(resultadoVisita))
            {
                ModelState.AddModelError(
                    "",
                    "Debe completar el resultado de la visita antes de cerrarla.");
            }

            if (!ModelState.IsValid)
            {
                return View(visita);
            }

            var solicitud =
                visita.SolicitudCotizacion;

            if (solicitud == null)
            {
                return NotFound();
            }

            // Guardar medidas definitivas
            var historial =
                new HistorialMedida
                {
                    SolicitudCotizacionId =
                        solicitud.Id,

                    Ancho =
                        ancho,

                    Alto =
                        alto,

                    Profundidad =
                        profundidad,

                    Cantidad =
                        cantidad,

                    UsuarioRegistro =
                        User.Identity?.Name,

                    FechaRegistro =
                        DateTime.Now,

                    Observaciones =
                        observaciones
                };

            _context.HistorialMedidas
                .Add(historial);

            // Guardar resultado de la visita
            visita.VisitaCompletada =
                true;

            visita.Estado =
                "Visita Técnica Completada";

            visita.ResultadoVisita =
                resultadoVisita;

            visita.Observaciones =
                observaciones;

            visita.FechaFinalizacion = DateTime.Now;

            solicitud.Estado = "Visita Técnica Completada";

            // Guardar fotografías de evidencia
            if (fotos != null && fotos.Count > 0)
            {
                var carpeta = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "visitas",
                    visita.Id.ToString());

                Directory.CreateDirectory(carpeta);

                foreach (var foto in fotos)
                {
                    if (foto == null || foto.Length == 0)
                        continue;

                    var extension = Path.GetExtension(foto.FileName)
                        .ToLowerInvariant();

                    var nombreArchivo = $"{Guid.NewGuid()}{extension}";

                    var rutaFisica = Path.Combine(
                        carpeta,
                        nombreArchivo);

                    await using var stream = new FileStream(
                        rutaFisica,
                        FileMode.Create);

                    await foto.CopyToAsync(stream);

                    var fotografia = new VisitaTecnicaFotografia
                    {
                        VisitaTecnicaId = visita.Id,
                        RutaArchivo = $"/uploads/visitas/{visita.Id}/{nombreArchivo}",
                        NombreOriginal = foto.FileName,
                        TipoContenido = foto.ContentType,
                        FechaCarga = DateTime.Now
                    };

                    _context.VisitaTecnicaFotografias.Add(fotografia);
                }
            }

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "El informe de la visita técnica se registró correctamente.";

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


        private async Task CargarTecnicos()
        {
            var tecnicos = await _userManager.GetUsersInRoleAsync("Tecnico");

            ViewBag.Tecnicos = tecnicos
                .Where(t => t.Activo)
                .OrderBy(t => t.NombreCompleto)
                .ToList();
        }

    }    
}
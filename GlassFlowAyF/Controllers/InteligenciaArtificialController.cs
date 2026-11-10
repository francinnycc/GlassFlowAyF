using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using GlassFlowAyF.Models.ViewModels;
using GlassFlowAyF.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers
{
    [Authorize]
    public class InteligenciaArtificialController
        : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser>
            _userManager;

        private readonly IOpenAIService
            _openAIService;

        private readonly IWebHostEnvironment
            _environment;


        public InteligenciaArtificialController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IOpenAIService openAIService,
            IWebHostEnvironment environment)
        {
            _context =
                context;

            _userManager =
                userManager;

            _openAIService =
                openAIService;

            _environment =
                environment;
        }


        // =====================================================
        // CLIENTE - PROYECTOS DISPONIBLES
        // =====================================================

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Index()
        {
            var usuario =
                await _userManager
                    .GetUserAsync(User);


            if (usuario == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }


            var solicitudes =
                await _context
                    .SolicitudesCotizacion
                    .AsNoTracking()
                    .Include(s =>
                        s.Producto)
                    .Include(s =>
                        s.Material)
                    .Include(s =>
                        s.Fotografias)
                    .Include(s =>
                        s.DisenosIA)
                    .Where(s =>
                        s.Correo ==
                        usuario.Email)
                    .OrderByDescending(
                        s =>
                            s.FechaSolicitud)
                    .ToListAsync();


            return View(
                solicitudes);
        }


        // =====================================================
        // GENERAR - GET
        // =====================================================

        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> Generar(
            int solicitudId)
        {
            var solicitud =
                await ObtenerSolicitudDelCliente(
                    solicitudId);


            if (solicitud == null)
            {
                return NotFound();
            }


            ViewBag.Solicitud =
                solicitud;

            ViewBag.Fotografias =
                solicitud.Fotografias;


            var model =
                new GenerarDisenoIAViewModel
                {
                    SolicitudId =
                        solicitud.Id,

                    FotografiaBaseId =
                        solicitud.Fotografias
                            .OrderBy(f =>
                                f.Id)
                            .Select(f =>
                                (int?)f.Id)
                            .FirstOrDefault(),

                    Estilo =
                        "Moderno",

                    ColorPerfil =
                        solicitud.Material?
                            .Perfil
                        ?? "Negro mate",

                    Ambiente =
                        "Elegante y luminoso"
                };


            return View(
                model);
        }


        // =====================================================
        // GENERAR - POST
        // =====================================================

        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generar(
            GenerarDisenoIAViewModel model,
            CancellationToken cancellationToken)
        {
            var solicitud =
                await ObtenerSolicitudDelCliente(
                    model.SolicitudId);


            if (solicitud == null)
            {
                return NotFound();
            }


            SolicitudFotografia?
                fotografiaBase =
                    null;


            if (model.FotografiaBaseId
                .HasValue)
            {
                fotografiaBase =
                    solicitud.Fotografias
                        .FirstOrDefault(
                            f =>
                                f.Id ==
                                model
                                    .FotografiaBaseId
                                    .Value);


                if (fotografiaBase == null)
                {
                    ModelState.AddModelError(
                        nameof(
                            model.FotografiaBaseId),

                        "La fotografía seleccionada " +
                        "no pertenece a esta solicitud.");
                }
            }


            if (!ModelState.IsValid)
            {
                ViewBag.Solicitud =
                    solicitud;

                ViewBag.Fotografias =
                    solicitud.Fotografias;


                return View(
                    model);
            }


            try
            {
                // RECOMENDACIÓN

                var recomendacion =
                    await _openAIService
                        .GenerarRecomendacionAsync(
                            solicitud,
                            fotografiaBase,
                            model.Estilo,
                            model.ColorPerfil,
                            model.Ambiente,
                            model.Preferencias,
                            cancellationToken);


                // IMAGEN

                var imagenBytes =
                    await _openAIService
                        .GenerarDisenoVisualAsync(
                            solicitud,
                            fotografiaBase,
                            model.Estilo,
                            model.ColorPerfil,
                            model.Ambiente,
                            model.Preferencias,
                            cancellationToken);


                // GUARDADO FÍSICO

                var rutaImagen =
                    await GuardarImagenGenerada(
                        solicitud.Id,
                        imagenBytes,
                        cancellationToken);


                var promptResumen =
                    $"{model.Estilo} | " +
                    $"{model.ColorPerfil} | " +
                    $"{model.Ambiente} | " +
                    $"{model.Preferencias}";


                // GUARDADO EN MYSQL

                var diseno =
                    new DisenoIA
                    {
                        SolicitudCotizacionId =
                            solicitud.Id,

                        FotografiaBaseId =
                            fotografiaBase?.Id,

                        Estilo =
                            model.Estilo,

                        ColorPerfil =
                            model.ColorPerfil,

                        Ambiente =
                            model.Ambiente,

                        Preferencias =
                            model.Preferencias,

                        Recomendacion =
                            recomendacion,

                        PromptGeneracion =
                            promptResumen,

                        RutaImagenGenerada =
                            rutaImagen,

                        ModeloTexto =
                            _openAIService
                                .ModeloTexto,

                        ModeloImagen =
                            _openAIService
                                .ModeloImagen,

                        FechaGeneracion =
                            DateTime.Now,

                        EsFavorito =
                            false
                    };


                _context.DisenosIA
                    .Add(diseno);


                await _context
                    .SaveChangesAsync(
                        cancellationToken);


                TempData["Mensaje"] =
                    "La propuesta de diseño con IA " +
                    "se generó correctamente.";


                return RedirectToAction(
                    nameof(Resultado),
                    new
                    {
                        id =
                            diseno.Id
                    });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);


                ViewBag.Solicitud =
                    solicitud;

                ViewBag.Fotografias =
                    solicitud.Fotografias;


                return View(
                    model);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,

                    "No fue posible comunicarse " +
                    "con el servicio de IA. " +
                    "Revise su conexión e intente nuevamente.");


                ViewBag.Solicitud =
                    solicitud;

                ViewBag.Fotografias =
                    solicitud.Fotografias;


                return View(
                    model);
            }
        }


        // =====================================================
        // RESULTADO
        // =====================================================

        [Authorize(
            Roles =
                "Cliente,Administrador")]
        public async Task<IActionResult> Resultado(
            int id)
        {
            var diseno =
                await _context
                    .DisenosIA
                    .AsNoTracking()

                    .Include(d =>
                        d.SolicitudCotizacion)

                        .ThenInclude(s =>
                            s!.Producto)

                    .Include(d =>
                        d.SolicitudCotizacion)

                        .ThenInclude(s =>
                            s!.Material)

                    .Include(d =>
                        d.FotografiaBase)
                    .Include(d => d.Revisiones)

                    .FirstOrDefaultAsync(
                        d =>
                            d.Id == id);


            if (diseno == null)
            {
                return NotFound();
            }


            if (User.IsInRole(
                "Cliente"))
            {
                var usuario =
                    await _userManager
                        .GetUserAsync(User);


                if (usuario == null ||
                    !string.Equals(diseno.SolicitudCotizacion?.Correo, usuario.Email, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }
            }


            return View(
                diseno);
        }


        // RESPUESTA FORMAL DEL CLIENTE A UNA PROPUESTA VISUAL
        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResponderDiseno(int id, string decision, string? comentario)
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null) return Unauthorized();

            var diseno = await _context.DisenosIA
                .Include(d => d.SolicitudCotizacion)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (diseno == null) return NotFound();
            if (!string.Equals(diseno.SolicitudCotizacion?.Correo, usuario.Email,
                StringComparison.OrdinalIgnoreCase)) return Forbid();

            var estados = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["aprobar"] = "Aprobado",
                ["rechazar"] = "Rechazado",
                ["cambios"] = "Cambios solicitados"
            };
            if (!estados.TryGetValue(decision ?? "", out var estado)) return BadRequest();
            comentario = comentario?.Trim();
            if (comentario?.Length > 2000)
            {
                TempData["Error"] = "El comentario no puede superar los 2000 caracteres.";
                return RedirectToAction(nameof(Resultado), new { id });
            }
            if (estado != "Aprobado" && string.IsNullOrWhiteSpace(comentario))
            {
                TempData["Error"] = "Indicá el motivo del rechazo o los cambios solicitados.";
                return RedirectToAction(nameof(Resultado), new { id });
            }
            if (diseno.EstadoRevision == "Aprobado" || diseno.EstadoRevision == "Rechazado")
            {
                TempData["Error"] = "Esta propuesta tiene una decisión definitiva y no admite nuevas respuestas.";
                return RedirectToAction(nameof(Resultado), new { id });
            }

            diseno.EstadoRevision = estado;
            diseno.ComentarioRevision = comentario;
            diseno.FechaUltimaRevision = DateTime.Now;
            diseno.UsuarioUltimaRevisionId = usuario.Id;
            _context.DisenosIARevisiones.Add(new DisenoIARevision
            {
                DisenoIAId = diseno.Id,
                Estado = estado,
                Comentario = comentario,
                UsuarioId = usuario.Id,
                Fecha = DateTime.Now
            });
            await _context.SaveChangesAsync();
            TempData["Success"] = "Respuesta del diseño registrada correctamente.";
            return RedirectToAction(nameof(Resultado), new { id });
        }

        // =====================================================
        // ADMINISTRADOR
        // =====================================================

        [Authorize(
            Roles =
                "Administrador")]
        public async Task<IActionResult>
            Administrar()
        {
            var disenos =
                await _context
                    .DisenosIA
                    .AsNoTracking()

                    .Include(d =>
                        d.SolicitudCotizacion)

                        .ThenInclude(s =>
                            s!.Producto)

                    .OrderByDescending(
                        d =>
                            d.EsFavorito)

                    .ThenByDescending(
                        d =>
                            d.FechaGeneracion)

                    .ToListAsync();


            return View(
                disenos);
        }


        // =====================================================
        // CLIENTE - MIS DISEÑOS
        // =====================================================

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult>
            MisDisenos()
        {
            var usuario =
                await _userManager
                    .GetUserAsync(User);


            if (usuario == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }


            var disenos =
                await _context
                    .DisenosIA
                    .AsNoTracking()
                    .Include(d => d.Revisiones)

                    .Include(d =>
                        d.SolicitudCotizacion)

                        .ThenInclude(s =>
                            s!.Producto)

                    .Where(d =>
                        d.SolicitudCotizacion != null &&
                        d.SolicitudCotizacion
                            .Correo ==
                        usuario.Email)

                    .OrderByDescending(
                        d =>
                            d.FechaGeneracion)

                    .ToListAsync();


            return View(
                disenos);
        }


        // =====================================================
        // FAVORITO
        // =====================================================

        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            SeleccionarFavorito(
                int id)
        {
            var usuario =
                await _userManager
                    .GetUserAsync(User);


            if (usuario == null)
            {
                return Unauthorized();
            }


            var diseno =
                await _context
                    .DisenosIA

                    .Include(d =>
                        d.SolicitudCotizacion)

                    .FirstOrDefaultAsync(
                        d =>
                            d.Id == id);


            if (diseno == null)
            {
                return NotFound();
            }


            if (!string.Equals(diseno.SolicitudCotizacion?.Correo, usuario.Email, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }


            // Solo una propuesta formalmente aprobada puede ser preferida.
            if (!string.Equals(diseno.EstadoRevision, "Aprobado", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Solo podés elegir como preferido un diseño aprobado.";
                return RedirectToAction(nameof(Resultado), new { id });
            }

            var disenosSolicitud =
                await _context
                    .DisenosIA

                    .Where(d =>
                        d.SolicitudCotizacionId ==
                        diseno.SolicitudCotizacionId)

                    .ToListAsync();


            foreach (var item
                in disenosSolicitud)
            {
                item.EsFavorito =
                    item.Id ==
                    diseno.Id;
            }


            await _context
                .SaveChangesAsync();


            TempData["Mensaje"] =
                "Diseño seleccionado como " +
                "preferido para esta solicitud.";


            return RedirectToAction(
                nameof(Resultado),
                new
                {
                    id =
                        diseno.Id
                });
        }


        // =====================================================
        // ELIMINAR DISEÑO
        // =====================================================

        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(
            int id)
        {
            var usuario =
                await _userManager
                    .GetUserAsync(User);


            if (usuario == null)
            {
                return Unauthorized();
            }


            var diseno =
                await _context
                    .DisenosIA

                    .Include(d =>
                        d.SolicitudCotizacion)

                    .FirstOrDefaultAsync(
                        d =>
                            d.Id == id);


            if (diseno == null)
            {
                return NotFound();
            }


            if (!string.Equals(diseno.SolicitudCotizacion?.Correo, usuario.Email, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }


            // Un diseño favorito o con respuestas formales no puede eliminarse:
            // preservamos la trazabilidad de las decisiones del cliente.
            var tieneRevisiones = await _context.DisenosIARevisiones
                .AnyAsync(r => r.DisenoIAId == diseno.Id);

            if (diseno.EsFavorito || tieneRevisiones ||
                !string.Equals(diseno.EstadoRevision, "Pendiente", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "No se puede eliminar un diseño preferido o con historial de revisión.";
                return RedirectToAction(nameof(MisDisenos));
            }

            // Guardar primero en la base de datos; después retirar el archivo.
            _context.DisenosIA.Remove(diseno);
            await _context.SaveChangesAsync();
            EliminarImagenGenerada(diseno.RutaImagenGenerada);


            TempData["Mensaje"] =
                "Diseño eliminado correctamente.";


            return RedirectToAction(
                nameof(MisDisenos));
        }


        // =====================================================
        // OBTENER SOLICITUD DEL CLIENTE
        // =====================================================

        private async
            Task<SolicitudCotizacion?>
            ObtenerSolicitudDelCliente(
                int solicitudId)
        {
            var usuario =
                await _userManager
                    .GetUserAsync(User);


            if (usuario == null)
            {
                return null;
            }


            return await _context
                .SolicitudesCotizacion

                .Include(s =>
                    s.Producto)

                .Include(s =>
                    s.Material)

                .Include(s =>
                    s.Fotografias)

                .Include(s =>
                    s.DisenosIA)

                .FirstOrDefaultAsync(
                    s =>
                        s.Id ==
                        solicitudId &&
                        s.Correo ==
                        usuario.Email);
        }


        // =====================================================
        // GUARDAR IMAGEN
        // =====================================================

        private async Task<string>
            GuardarImagenGenerada(
                int solicitudId,
                byte[] bytes,
                CancellationToken cancellationToken)
        {
            var carpeta =
                Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "disenos-ia",
                    solicitudId.ToString());


            Directory.CreateDirectory(
                carpeta);


            var nombreArchivo =
                $"{Guid.NewGuid()}.jpg";


            var rutaFisica =
                Path.Combine(
                    carpeta,
                    nombreArchivo);


            await System.IO.File
                .WriteAllBytesAsync(
                    rutaFisica,
                    bytes,
                    cancellationToken);


            return
                $"/uploads/disenos-ia/" +
                $"{solicitudId}/" +
                $"{nombreArchivo}";
        }


        // =====================================================
        // ELIMINAR IMAGEN
        // =====================================================

        private void EliminarImagenGenerada(
            string? rutaImagen)
        {
            if (string.IsNullOrWhiteSpace(
                rutaImagen))
            {
                return;
            }


            var rutaRelativa =
                rutaImagen
                    .TrimStart('/')
                    .Replace(
                        '/',
                        Path.DirectorySeparatorChar);


            var rutaFisica =
                Path.Combine(
                    _environment.WebRootPath,
                    rutaRelativa);


            if (System.IO.File.Exists(
                rutaFisica))
            {
                System.IO.File.Delete(
                    rutaFisica);
            }
        }
    }
}
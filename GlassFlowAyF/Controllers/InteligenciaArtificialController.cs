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
                    diseno
                        .SolicitudCotizacion?
                        .Correo !=
                    usuario.Email)
                {
                    return Forbid();
                }
            }


            return View(
                diseno);
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


            if (diseno
                .SolicitudCotizacion?
                .Correo !=
                usuario.Email)
            {
                return Forbid();
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


            if (diseno
                .SolicitudCotizacion?
                .Correo !=
                usuario.Email)
            {
                return Forbid();
            }


            EliminarImagenGenerada(
                diseno
                    .RutaImagenGenerada);


            _context.DisenosIA
                .Remove(diseno);


            await _context
                .SaveChangesAsync();


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
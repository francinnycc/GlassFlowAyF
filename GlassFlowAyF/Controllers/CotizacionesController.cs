using ClosedXML.Excel;
using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers
{
    [Authorize]
    public partial class CotizacionesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public CotizacionesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // =========================================================
        // ADMINISTRADOR - TODAS LAS SOLICITUDES
        // =========================================================

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            var solicitudes =
                await _context.SolicitudesCotizacion
                    .AsNoTracking()
                    .Include(s => s.Producto)
                    .Include(s => s.Material)
                    .Include(s => s.Cotizaciones)
                    .OrderByDescending(s => s.FechaSolicitud)
                    .ToListAsync();

            return View(solicitudes);
        }

        // =========================================================
        // CLIENTE - MIS SOLICITUDES
        // =========================================================

        [Authorize(Roles = "Cliente")]
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
                await _context.SolicitudesCotizacion
                    .AsNoTracking()
                    .Include(s => s.Producto)
                    .Include(s => s.Material)
                    .Include(s => s.Cotizaciones)
                    .Where(s => s.Correo == usuario.Email)
                    .OrderByDescending(s => s.FechaSolicitud)
                    .ToListAsync();

            return View(solicitudes);
        }

        // =========================================================
        // CREAR SOLICITUD - GET
        // =========================================================

        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> Create(
            int? productoId)
        {
            var usuario =
                await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var model =
                new SolicitudCotizacion
                {
                    NombreCliente =
                        usuario.NombreCompleto,

                    Correo =
                        usuario.Email ?? string.Empty,

                    Telefono =
                        usuario.PhoneNumber ?? string.Empty,

                    Direccion =
                        usuario.Direccion,

                    ProductoId =
                        productoId,

                    Cantidad = 1
                };

            await CargarCombos(
                productoId,
                null);

            return View(model);
        }

        // =========================================================
        // CREAR SOLICITUD - POST
        // =========================================================

        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ProductoId,MaterialId,Descripcion,Ancho,Alto,Profundidad,Cantidad,RequiereInstalacion,Direccion,Observaciones")] SolicitudCotizacion solicitud,
            List<IFormFile>? fotos,
            string accion = "enviar")
        {
            var usuario =
                await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            // Datos reales del usuario autenticado.
            solicitud.NombreCliente =
                usuario.NombreCompleto;

            solicitud.Correo =
                usuario.Email ?? string.Empty;

            solicitud.Telefono =
                usuario.PhoneNumber ?? string.Empty;

            // =====================================================
            // GUARDAR COMO BORRADOR
            // =====================================================

            if (accion == "borrador")
            {
                Producto? productoBorrador = null;

                if (solicitud.ProductoId.HasValue)
                {
                    productoBorrador =
                        await _context.Productos
                            .FirstOrDefaultAsync(p =>
                                p.Id ==
                                    solicitud.ProductoId.Value &&
                                p.Activo);
                }

                solicitud.TipoProducto =
                    productoBorrador?.Nombre ??
                    string.Empty;

                solicitud.Descripcion ??=
                    string.Empty;

                if (solicitud.Cantidad <= 0)
                {
                    solicitud.Cantidad = 1;
                }

                solicitud.FechaSolicitud =
                    DateTime.Now;

                solicitud.Estado =
                    "Borrador";

                // Validación de fotografías del borrador.
                if (fotos != null &&
                    fotos.Count(f => f.Length > 0) > 6)
                {
                    TempData["Error"] =
                        "Puede subir un máximo de 6 fotografías.";

                    await CargarCombos(
                        solicitud.ProductoId,
                        solicitud.MaterialId);

                    return View(solicitud);
                }

                if (fotos != null)
                {
                    foreach (var foto in fotos)
                    {
                        if (foto.Length == 0)
                        {
                            continue;
                        }

                        if (!FotografiaValida(foto))
                        {
                            TempData["Error"] =
                                $"La fotografía '{foto.FileName}' no es válida. Use JPG, JPEG, PNG o WEBP, máximo 5 MB.";

                            await CargarCombos(
                                solicitud.ProductoId,
                                solicitud.MaterialId);

                            return View(solicitud);
                        }
                    }
                }

                _context.SolicitudesCotizacion
                    .Add(solicitud);

                await _context.SaveChangesAsync();

                if (fotos != null)
                {
                    foreach (var foto in fotos)
                    {
                        if (foto.Length == 0)
                        {
                            continue;
                        }

                        var ruta =
                            await GuardarFotografia(
                                solicitud.Id,
                                foto);

                        _context.SolicitudFotografias.Add(
                            new SolicitudFotografia
                            {
                                SolicitudCotizacionId =
                                    solicitud.Id,

                                RutaArchivo =
                                    ruta,

                                NombreOriginal =
                                    foto.FileName,

                                TipoContenido =
                                    foto.ContentType,

                                FechaCarga =
                                    DateTime.Now
                            });
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["Mensaje"] =
                    $"Borrador #SOL-{solicitud.Id:D5} guardado correctamente.";

                return RedirectToAction(
                    nameof(MisSolicitudes));
            }

            // =====================================================
            // VALIDACIONES PARA ENVIAR
            // =====================================================

            if (!solicitud.ProductoId.HasValue)
            {
                ModelState.AddModelError(
                    nameof(solicitud.ProductoId),
                    "Debe seleccionar un producto.");
            }

            if (!solicitud.MaterialId.HasValue)
            {
                ModelState.AddModelError(
                    nameof(solicitud.MaterialId),
                    "Debe seleccionar un material o acabado.");
            }

            if (!solicitud.Ancho.HasValue ||
                !solicitud.Alto.HasValue)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Debe ingresar el ancho y alto aproximados.");
            }

            if (solicitud.RequiereInstalacion &&
                string.IsNullOrWhiteSpace(
                    solicitud.Direccion))
            {
                ModelState.AddModelError(
                    nameof(solicitud.Direccion),
                    "Debe indicar la dirección donde se realizará la instalación.");
            }

            if (fotos != null &&
                fotos.Count(f => f.Length > 0) > 6)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Puede subir un máximo de 6 fotografías.");
            }

            if (fotos != null)
            {
                foreach (var foto in fotos)
                {
                    if (foto.Length == 0)
                    {
                        continue;
                    }

                    if (!FotografiaValida(foto))
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            $"La fotografía '{foto.FileName}' no es válida. Use JPG, JPEG, PNG o WEBP, máximo 5 MB.");
                    }
                }
            }

            ModelState.Remove(
                nameof(solicitud.NombreCliente));

            ModelState.Remove(
                nameof(solicitud.Correo));

            ModelState.Remove(
                nameof(solicitud.Telefono));

            ModelState.Remove(
                nameof(solicitud.TipoProducto));

            if (!ModelState.IsValid)
            {
                await CargarCombos(
                    solicitud.ProductoId,
                    solicitud.MaterialId);

                return View(solicitud);
            }

            var producto =
                await _context.Productos
                    .FirstOrDefaultAsync(p =>
                        p.Id ==
                            solicitud.ProductoId &&
                        p.Activo);

            if (producto == null)
            {
                ModelState.AddModelError(
                    nameof(solicitud.ProductoId),
                    "El producto seleccionado no está disponible.");

                await CargarCombos(
                    solicitud.ProductoId,
                    solicitud.MaterialId);

                return View(solicitud);
            }

            var materialPermitido =
                await _context.ProductoMateriales
                    .AnyAsync(pm =>
                        pm.ProductoId ==
                            solicitud.ProductoId &&
                        pm.MaterialId ==
                            solicitud.MaterialId &&
                        pm.Material != null &&
                        pm.Material.Activo);

            if (!materialPermitido)
            {
                ModelState.AddModelError(
                    nameof(solicitud.MaterialId),
                    "El material seleccionado no está disponible para este producto.");

                await CargarCombos(
                    solicitud.ProductoId,
                    solicitud.MaterialId);

                return View(solicitud);
            }

            solicitud.TipoProducto =
                producto.Nombre;

            solicitud.FechaSolicitud =
                DateTime.Now;

            solicitud.Estado =
                "Solicitado";

            _context.SolicitudesCotizacion
                .Add(solicitud);

            await _context.SaveChangesAsync();

            if (fotos != null)
            {
                foreach (var foto in fotos)
                {
                    if (foto.Length == 0)
                    {
                        continue;
                    }

                    var ruta =
                        await GuardarFotografia(
                            solicitud.Id,
                            foto);

                    _context.SolicitudFotografias.Add(
                        new SolicitudFotografia
                        {
                            SolicitudCotizacionId =
                                solicitud.Id,

                            RutaArchivo =
                                ruta,

                            NombreOriginal =
                                foto.FileName,

                            TipoContenido =
                                foto.ContentType,

                            FechaCarga =
                                DateTime.Now
                        });
                }

                await _context.SaveChangesAsync();
            }

            TempData["Mensaje"] =
                $"Solicitud #SOL-{solicitud.Id:D5} registrada correctamente.";

            return RedirectToAction(
                nameof(Details),
                new
                {
                    id = solicitud.Id
                });
        }

        // =========================================================
        // EDITAR SOLICITUD - GET
        // =========================================================

        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> Edit(
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
                await _context.SolicitudesCotizacion
                    .FirstOrDefaultAsync(s =>
                        s.Id == id &&
                        s.Correo == usuario.Email);

            if (solicitud == null)
            {
                return NotFound();
            }

            if (solicitud.Estado != "Borrador" &&
                solicitud.Estado != "Solicitado")
            {
                TempData["Error"] =
                    "La solicitud ya está siendo procesada y no puede editarse.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }

            await CargarCombos(
                solicitud.ProductoId,
                solicitud.MaterialId);

            return View(solicitud);
        }

        // =========================================================
        // EDITAR SOLICITUD - POST
        // =========================================================

        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ProductoId,MaterialId,Descripcion,Ancho,Alto,Profundidad,Cantidad,RequiereInstalacion,Direccion,Observaciones")] SolicitudCotizacion model,
            List<IFormFile>? fotos,
            string accion = "enviar")
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
                await _context.SolicitudesCotizacion
                    .FirstOrDefaultAsync(s =>
                        s.Id == id &&
                        s.Correo == usuario.Email);

            if (solicitud == null)
            {
                return NotFound();
            }

            if (solicitud.Estado != "Borrador" &&
                solicitud.Estado != "Solicitado")
            {
                TempData["Error"] =
                    "La solicitud ya está siendo procesada y no puede editarse.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }

            // =====================================================
            // GUARDAR NUEVAMENTE COMO BORRADOR
            // =====================================================

            if (accion == "borrador")
            {
                Producto? producto = null;

                if (model.ProductoId.HasValue)
                {
                    producto =
                        await _context.Productos
                            .FirstOrDefaultAsync(p =>
                                p.Id ==
                                    model.ProductoId.Value &&
                                p.Activo);
                }

                solicitud.ProductoId =
                    model.ProductoId;

                solicitud.MaterialId =
                    model.MaterialId;

                solicitud.TipoProducto =
                    producto?.Nombre ??
                    string.Empty;

                solicitud.Descripcion =
                    model.Descripcion ??
                    string.Empty;

                solicitud.Ancho =
                    model.Ancho;

                solicitud.Alto =
                    model.Alto;

                solicitud.Profundidad =
                    model.Profundidad;

                solicitud.Cantidad =
                    model.Cantidad <= 0
                        ? 1
                        : model.Cantidad;

                solicitud.RequiereInstalacion =
                    model.RequiereInstalacion;

                solicitud.Direccion =
                    model.Direccion;

                solicitud.Observaciones =
                    model.Observaciones;

                solicitud.Estado =
                    "Borrador";

                if (fotos != null &&
                    fotos.Count(f => f.Length > 0) > 6)
                {
                    TempData["Error"] =
                        "Puede subir un máximo de 6 fotografías por actualización.";

                    await CargarCombos(
                        model.ProductoId,
                        model.MaterialId);

                    return View(model);
                }

                if (fotos != null)
                {
                    foreach (var foto in fotos)
                    {
                        if (foto.Length == 0)
                        {
                            continue;
                        }

                        if (!FotografiaValida(foto))
                        {
                            TempData["Error"] =
                                $"La fotografía '{foto.FileName}' no es válida. Use JPG, JPEG, PNG o WEBP, máximo 5 MB.";

                            await CargarCombos(
                                model.ProductoId,
                                model.MaterialId);

                            return View(model);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                if (fotos != null)
                {
                    foreach (var foto in fotos)
                    {
                        if (foto.Length == 0)
                        {
                            continue;
                        }

                        var ruta =
                            await GuardarFotografia(
                                solicitud.Id,
                                foto);

                        _context.SolicitudFotografias.Add(
                            new SolicitudFotografia
                            {
                                SolicitudCotizacionId =
                                    solicitud.Id,

                                RutaArchivo =
                                    ruta,

                                NombreOriginal =
                                    foto.FileName,

                                TipoContenido =
                                    foto.ContentType,

                                FechaCarga =
                                    DateTime.Now
                            });
                    }

                    await _context.SaveChangesAsync();
                }

                TempData["Mensaje"] =
                    "Borrador actualizado correctamente.";

                return RedirectToAction(
                    nameof(MisSolicitudes));
            }

            // =====================================================
            // VALIDACIONES PARA ENVIAR
            // =====================================================

            if (!model.ProductoId.HasValue)
            {
                ModelState.AddModelError(
                    nameof(model.ProductoId),
                    "Debe seleccionar un producto.");
            }

            if (!model.MaterialId.HasValue)
            {
                ModelState.AddModelError(
                    nameof(model.MaterialId),
                    "Debe seleccionar un material o acabado.");
            }

            if (!model.Ancho.HasValue ||
                !model.Alto.HasValue)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Debe ingresar el ancho y alto aproximados.");
            }

            if (model.RequiereInstalacion &&
                string.IsNullOrWhiteSpace(
                    model.Direccion))
            {
                ModelState.AddModelError(
                    nameof(model.Direccion),
                    "Debe indicar la dirección donde se realizará la instalación.");
            }

            if (fotos != null &&
                fotos.Count(f => f.Length > 0) > 6)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Puede subir un máximo de 6 fotografías por actualización.");
            }

            if (fotos != null)
            {
                foreach (var foto in fotos)
                {
                    if (foto.Length == 0)
                    {
                        continue;
                    }

                    if (!FotografiaValida(foto))
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            $"La fotografía '{foto.FileName}' no es válida. Use JPG, JPEG, PNG o WEBP, máximo 5 MB.");
                    }
                }
            }

            ModelState.Remove(
                nameof(model.NombreCliente));

            ModelState.Remove(
                nameof(model.Correo));

            ModelState.Remove(
                nameof(model.Telefono));

            ModelState.Remove(
                nameof(model.TipoProducto));

            if (!ModelState.IsValid)
            {
                await CargarCombos(
                    model.ProductoId,
                    model.MaterialId);

                return View(model);
            }

            var productoSeleccionado =
                await _context.Productos
                    .FirstOrDefaultAsync(p =>
                        p.Id ==
                            model.ProductoId &&
                        p.Activo);

            if (productoSeleccionado == null)
            {
                ModelState.AddModelError(
                    nameof(model.ProductoId),
                    "El producto seleccionado no está disponible.");

                await CargarCombos(
                    model.ProductoId,
                    model.MaterialId);

                return View(model);
            }

            var materialPermitido =
                await _context.ProductoMateriales
                    .AnyAsync(pm =>
                        pm.ProductoId ==
                            model.ProductoId &&
                        pm.MaterialId ==
                            model.MaterialId &&
                        pm.Material != null &&
                        pm.Material.Activo);

            if (!materialPermitido)
            {
                ModelState.AddModelError(
                    nameof(model.MaterialId),
                    "El material seleccionado no está disponible para este producto.");

                await CargarCombos(
                    model.ProductoId,
                    model.MaterialId);

                return View(model);
            }

            solicitud.ProductoId =
                model.ProductoId;

            solicitud.MaterialId =
                model.MaterialId;

            solicitud.TipoProducto =
                productoSeleccionado.Nombre;

            solicitud.Descripcion =
                model.Descripcion;

            solicitud.Ancho =
                model.Ancho;

            solicitud.Alto =
                model.Alto;

            solicitud.Profundidad =
                model.Profundidad;

            solicitud.Cantidad =
                model.Cantidad;

            solicitud.RequiereInstalacion =
                model.RequiereInstalacion;

            solicitud.Direccion =
                model.Direccion;

            solicitud.Observaciones =
                model.Observaciones;

            solicitud.Estado =
                "Solicitado";

            await _context.SaveChangesAsync();

            // Fotografías nuevas agregadas durante edición.
            if (fotos != null)
            {
                foreach (var foto in fotos)
                {
                    if (foto.Length == 0)
                    {
                        continue;
                    }

                    var ruta =
                        await GuardarFotografia(
                            solicitud.Id,
                            foto);

                    _context.SolicitudFotografias.Add(
                        new SolicitudFotografia
                        {
                            SolicitudCotizacionId =
                                solicitud.Id,

                            RutaArchivo =
                                ruta,

                            NombreOriginal =
                                foto.FileName,

                            TipoContenido =
                                foto.ContentType,

                            FechaCarga =
                                DateTime.Now
                        });
                }

                await _context.SaveChangesAsync();
            }

            TempData["Mensaje"] =
                $"Solicitud #SOL-{solicitud.Id:D5} actualizada y enviada correctamente.";

            return RedirectToAction(
                nameof(Details),
                new { id = solicitud.Id });
        }

        // =========================================================
        // DETALLE DE SOLICITUD
        // =========================================================

        [Authorize(Roles = "Administrador,Cliente")]
        public async Task<IActionResult> Details(
            int id)
        {
            var solicitud =
                await _context.SolicitudesCotizacion
                    .AsNoTracking()
                    .Include(s => s.Producto)
                    .Include(s => s.Material)
                    .Include(s => s.Fotografias)
                    .Include(s => s.ValidacionesTecnicas)
                    .Include(s => s.Cotizaciones)
                        .ThenInclude(c => c.Compra)
                    .FirstOrDefaultAsync(
                        s => s.Id == id);

            if (solicitud == null)
            {
                return NotFound();
            }

            // Un cliente solo puede ver sus solicitudes.
            if (User.IsInRole("Cliente"))
            {
                var usuario =
                    await _userManager
                        .GetUserAsync(User);

                if (usuario == null ||
                    solicitud.Correo !=
                        usuario.Email)
                {
                    return Forbid();
                }
            }

            return View(solicitud);
        }

        // =========================================================
        // CAMBIAR ESTADO MANUALMENTE
        // =========================================================

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(
            int id,
            string estado)
        {
            var solicitud =
                await _context.SolicitudesCotizacion
                    .FindAsync(id);

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

            solicitud.Estado =
                estado;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Estado actualizado correctamente.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }

        // =========================================================
        // GENERAR COTIZACIÓN - GET
        // =========================================================

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> CrearCotizacion(
            int solicitudId)
        {
            var solicitud =
                await _context.SolicitudesCotizacion
                    .Include(s => s.Producto)
                    .Include(s => s.Material)
                    .Include(s => s.Cotizaciones)
                    .FirstOrDefaultAsync(
                        s => s.Id == solicitudId);

            if (solicitud == null)
            {
                return NotFound();
            }

            var existente =
                solicitud.Cotizaciones
                    .Where(c =>
                        c.Estado != "Rechazada")
                    .OrderByDescending(
                        c => c.FechaCreacion)
                    .FirstOrDefault();

            if (existente != null)
            {
                TempData["Error"] =
                    "La solicitud ya posee una cotización activa.";

                return RedirectToAction(
                    nameof(VerCotizacion),
                    new
                    {
                        id = existente.Id
                    });
            }

            var precioProducto =
                solicitud.Producto?
                    .PrecioBase ?? 0m;

            var adicionalMaterial =
                solicitud.Material?
                    .PrecioAdicional ?? 0m;

            var cotizacion =
                new Cotizacion
                {
                    SolicitudCotizacionId =
                        solicitud.Id,

                    CostoMateriales =
                        (precioProducto + adicionalMaterial) * solicitud.Cantidad,

                    ManoObra =
                        35000m,

                    CostoInstalacion =
                        solicitud.RequiereInstalacion
                            ? 50000m
                            : 0m,

                    OtrosCostos =
                        0m,

                    PorcentajeImpuesto =
                        13m,

                    Descuento =
                        0m,

                    FechaCreacion =
                        DateTime.Now,

                    FechaVencimiento =
                        DateTime.Now.AddDays(15),

                    DetalleTecnico =
                        $"Producto: {solicitud.TipoProducto}. " +
                        $"Medidas aproximadas: " +
                        $"{solicitud.Ancho?.ToString("0.00")} m × " +
                        $"{solicitud.Alto?.ToString("0.00")} m.",

                    Condiciones =
                        "Cotización sujeta a validación técnica de medidas, disponibilidad de materiales y condiciones del sitio."
                };

            CalcularCotizacion(cotizacion);

            ViewBag.Solicitud =
                solicitud;

            return View(cotizacion);
        }

        // =========================================================
        // GENERAR COTIZACIÓN - POST
        // =========================================================

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCotizacion(
            [Bind("SolicitudCotizacionId,CostoMateriales,ManoObra,CostoInstalacion,OtrosCostos,PorcentajeImpuesto,Descuento,FechaVencimiento,DetalleTecnico,Condiciones")] Cotizacion cotizacion)
        {
            ValidarImportes(cotizacion);
            if (cotizacion.FechaVencimiento <
                DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(cotizacion.FechaVencimiento),
                    "La fecha de vencimiento debe ser igual o posterior a hoy.");
            }

            var solicitud =
                await _context.SolicitudesCotizacion
                    .Include(s => s.Cotizaciones)
                    .FirstOrDefaultAsync(s =>
                        s.Id ==
                            cotizacion.SolicitudCotizacionId);

            if (solicitud == null)
            {
                return NotFound();
            }

            var yaExiste =
                solicitud.Cotizaciones
                    .Any(c =>
                        c.Estado != "Rechazada");

            if (yaExiste)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Esta solicitud ya posee una cotización activa.");
            }

            CalcularCotizacion(
                cotizacion);

            if (!ModelState.IsValid)
            {
                ViewBag.Solicitud =
                    solicitud;

                return View(cotizacion);
            }

            cotizacion.Estado =
                "Pendiente";

            cotizacion.FechaCreacion =
                DateTime.Now;

            _context.Cotizaciones
                .Add(cotizacion);

            solicitud.Estado =
                "Cotizado";

            solicitud.MontoEstimado =
                cotizacion.Total;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Cotización generada y enviada al cliente correctamente.";

            return RedirectToAction(
                nameof(VerCotizacion),
                new
                {
                    id = cotizacion.Id
                });
        }

        // =========================================================
        // VER COTIZACIÓN
        // =========================================================

        [Authorize]
        public async Task<IActionResult> VerCotizacion(
            int id)
        {
            var cotizacion =
                await _context.Cotizaciones
                    .AsNoTracking()
                    .Include(c =>
                        c.SolicitudCotizacion)
                        .ThenInclude(s =>
                            s!.Producto)
                    .Include(c =>
                        c.SolicitudCotizacion)
                        .ThenInclude(s =>
                            s!.Material)
                    .Include(c => c.Compra)
                    .FirstOrDefaultAsync(
                        c => c.Id == id);

            if (cotizacion == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Cliente"))
            {
                var usuario =
                    await _userManager
                        .GetUserAsync(User);

                if (usuario == null ||
                    cotizacion
                        .SolicitudCotizacion?
                        .Correo != usuario.Email)
                {
                    return Forbid();
                }
            }

            return View(cotizacion);
        }

        // =========================================================
        // RESPUESTA DEL CLIENTE
        // =========================================================

        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResponderCotizacion(
            int id,
            string respuesta,
            string? comentario,
            Guid version)
        {
            var usuario =
                await _userManager
                    .GetUserAsync(User);

            if (usuario == null)
            {
                return Unauthorized();
            }

            var cotizacion =
                await _context.Cotizaciones
                    .Include(c =>
                        c.SolicitudCotizacion)
                    .FirstOrDefaultAsync(
                        c => c.Id == id);

            if (cotizacion == null)
            {
                return NotFound();
            }

            if (cotizacion
                .SolicitudCotizacion?
                .Correo != usuario.Email)
            {
                return Forbid();
            }

            if (version != cotizacion.Version) return Conflict("La cotización cambió. Recárguela antes de responder.");
            if (comentario?.Length > 1000) return BadRequest("El comentario no puede exceder 1000 caracteres.");

            if (cotizacion.Estado !=
                "Pendiente")
            {
                TempData["Error"] =
                    "Esta cotización ya fue respondida.";

                return RedirectToAction(
                    nameof(VerCotizacion),
                    new { id });
            }

            if (cotizacion.FechaVencimiento <
                DateTime.Today)
            {
                TempData["Error"] =
                    "Esta cotización ha vencido.";

                return RedirectToAction(
                    nameof(VerCotizacion),
                    new { id });
            }

            cotizacion.ComentarioCliente =
                comentario;

            cotizacion.FechaRespuestaCliente =
                DateTime.Now;

            switch (respuesta)
            {
                case "Aprobar":

                    cotizacion.Estado =
                        "Aprobada";

                    cotizacion
                        .SolicitudCotizacion!
                        .Estado =
                            "Aprobado";

                    break;

                case "Cambios":

                    cotizacion.Estado =
                        "Cambios solicitados";

                    cotizacion
                        .SolicitudCotizacion!
                        .Estado =
                            "En revisión";

                    break;

                case "Rechazar":

                    cotizacion.Estado =
                        "Rechazada";

                    break;

                default:
                    return BadRequest();
            }

            cotizacion.Version = Guid.NewGuid();
            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException) { return Conflict("La cotización cambió. Recárguela antes de responder."); }

            TempData["Mensaje"] = "Respuesta registrada correctamente.";

            return RedirectToAction(
                nameof(VerCotizacion),
                new { id });
        }

        // =========================================================
        // EXPORTAR COTIZACIÓN EXCEL
        // =========================================================

        [Authorize]
        public async Task<IActionResult> ExportarCotizacionExcel(
            int id)
        {
            var cotizacion =
                await _context.Cotizaciones
                    .AsNoTracking()
                    .Include(c =>
                        c.SolicitudCotizacion)
                        .ThenInclude(s =>
                            s!.Producto)
                    .Include(c =>
                        c.SolicitudCotizacion)
                        .ThenInclude(s =>
                            s!.Material)
                    .FirstOrDefaultAsync(
                        c => c.Id == id);

            if (cotizacion == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Cliente"))
            {
                var usuario =
                    await _userManager
                        .GetUserAsync(User);

                if (usuario == null ||
                    cotizacion
                        .SolicitudCotizacion?
                        .Correo != usuario.Email)
                {
                    return Forbid();
                }
            }

            using var workbook =
                new XLWorkbook();

            var hoja =
                workbook.Worksheets.Add(
                    "Cotización");

            // =====================================================
            // ENCABEZADO
            // =====================================================

            hoja.Cell("A1").Value =
                "GLASSFLOW A&F";

            hoja.Cell("A2").Value =
                "Diseño • Calidad • Instalación";

            hoja.Cell("A4").Value =
                "COTIZACIÓN";

            hoja.Cell("A5").Value =
                "Número";

            hoja.Cell("B5").Value =
                $"COT-{cotizacion.Id:D5}";

            hoja.Cell("A6").Value =
                "Fecha";

            hoja.Cell("B6").Value =
                cotizacion.FechaCreacion;

            hoja.Cell("A7").Value =
                "Válida hasta";

            hoja.Cell("B7").Value =
                cotizacion.FechaVencimiento;

            // =====================================================
            // CLIENTE
            // =====================================================

            hoja.Cell("A9").Value =
                "CLIENTE";

            hoja.Cell("A10").Value =
                "Nombre";

            hoja.Cell("B10").Value =
                cotizacion
                    .SolicitudCotizacion?
                    .NombreCliente ?? "";

            hoja.Cell("A11").Value =
                "Correo";

            hoja.Cell("B11").Value =
                cotizacion
                    .SolicitudCotizacion?
                    .Correo ?? "";

            hoja.Cell("A12").Value =
                "Teléfono";

            hoja.Cell("B12").Value =
                cotizacion
                    .SolicitudCotizacion?
                    .Telefono ?? "";

            // =====================================================
            // PROYECTO
            // =====================================================

            hoja.Cell("A14").Value =
                "PROYECTO";

            hoja.Cell("A15").Value =
                "Producto";

            hoja.Cell("B15").Value =
                cotizacion
                    .SolicitudCotizacion?
                    .Producto?
                    .Nombre
                ??
                cotizacion
                    .SolicitudCotizacion?
                    .TipoProducto
                ?? "";

            hoja.Cell("A16").Value =
                "Material";

            hoja.Cell("B16").Value =
                cotizacion
                    .SolicitudCotizacion?
                    .Material?
                    .Nombre
                ?? "No especificado";

            // =====================================================
            // COSTOS
            // =====================================================

            hoja.Cell("A18").Value =
                "Concepto";

            hoja.Cell("B18").Value =
                "Monto";

            hoja.Cell("A19").Value =
                "Materiales";

            hoja.Cell("B19").Value =
                cotizacion.CostoMateriales;

            hoja.Cell("A20").Value =
                "Mano de obra";

            hoja.Cell("B20").Value =
                cotizacion.ManoObra;

            hoja.Cell("A21").Value =
                "Instalación";

            hoja.Cell("B21").Value =
                cotizacion.CostoInstalacion;

            hoja.Cell("A22").Value =
                "Otros costos";

            hoja.Cell("B22").Value =
                cotizacion.OtrosCostos;

            hoja.Cell("A24").Value =
                "Subtotal";

            hoja.Cell("B24").Value =
                cotizacion.Subtotal;

            hoja.Cell("A25").Value =
                $"Impuesto ({cotizacion.PorcentajeImpuesto}%)";

            hoja.Cell("B25").Value =
                cotizacion.Impuesto;

            hoja.Cell("A26").Value =
                "Descuento";

            hoja.Cell("B26").Value =
                cotizacion.Descuento;

            hoja.Cell("A27").Value =
                "TOTAL";

            hoja.Cell("B27").Value =
                cotizacion.Total;

            // =====================================================
            // DETALLE
            // =====================================================

            hoja.Cell("A29").Value =
                "Detalle técnico";

            hoja.Cell("B29").Value =
                cotizacion.DetalleTecnico ?? "";

            hoja.Cell("A30").Value =
                "Condiciones";

            hoja.Cell("B30").Value =
                cotizacion.Condiciones ?? "";

            // =====================================================
            // ESTILO
            // =====================================================

            hoja.Range("A1:B1")
                .Merge();

            hoja.Range("A2:B2")
                .Merge();

            hoja.Range("A4:B4")
                .Merge();

            hoja.Cell("A1")
                .Style.Font.Bold = true;

            hoja.Cell("A1")
                .Style.Font.FontSize = 20;

            hoja.Cell("A4")
                .Style.Font.Bold = true;

            hoja.Cell("A4")
                .Style.Font.FontSize = 16;

            hoja.Range("A18:B18")
                .Style.Font.Bold = true;

            hoja.Range("A27:B27")
                .Style.Font.Bold = true;

            hoja.Range("B19:B27")
                .Style.NumberFormat
                .Format =
                    "₡#,##0.00";

            hoja.Column("A").Width = 25;
            hoja.Column("B").Width = 55;

            hoja.Rows()
                .AdjustToContents();

            using var stream =
                new MemoryStream();

            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Cotizacion_COT-{cotizacion.Id:D5}.xlsx");
        }

        // =========================================================
        // COMBOS
        // =========================================================

        private async Task CargarCombos(
            int? productoSeleccionado,
            int? materialSeleccionado)
        {
            var productos =
                await _context.Productos
                    .Where(p => p.Activo)
                    .OrderBy(p => p.Nombre)
                    .ToListAsync();

            ViewBag.Productos =
                new SelectList(
                    productos,
                    "Id",
                    "Nombre",
                    productoSeleccionado);

            var materiales =
                new List<Material>();

            if (productoSeleccionado.HasValue)
            {
                materiales =
                    await _context.ProductoMateriales
                        .Where(pm =>
                            pm.ProductoId ==
                                productoSeleccionado &&
                            pm.Material != null &&
                            pm.Material.Activo)
                        .Select(pm =>
                            pm.Material!)
                        .OrderBy(m =>
                            m.Nombre)
                        .ToListAsync();
            }

            ViewBag.Materiales =
                new SelectList(
                    materiales,
                    "Id",
                    "Nombre",
                    materialSeleccionado);
        }

        // =========================================================
        // VALIDAR FOTOGRAFÍA
        // =========================================================

        private static bool FotografiaValida(
            IFormFile foto)
        {
            string[] extensiones =
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            var extension =
                Path.GetExtension(
                    foto.FileName)
                    .ToLowerInvariant();

            return
                extensiones.Contains(extension) &&
                foto.Length > 0 &&
                foto.Length <=
                    5 * 1024 * 1024;
        }

        // =========================================================
        // GUARDAR FOTOGRAFÍA
        // =========================================================

        private async Task<string> GuardarFotografia(
            int solicitudId,
            IFormFile foto)
        {
            var extension =
                Path.GetExtension(
                    foto.FileName)
                    .ToLowerInvariant();

            var carpeta =
                Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "solicitudes",
                    solicitudId.ToString());

            Directory.CreateDirectory(
                carpeta);

            var nombreArchivo =
                $"{Guid.NewGuid()}{extension}";

            var rutaFisica =
                Path.Combine(
                    carpeta,
                    nombreArchivo);

            await using var stream =
                new FileStream(
                    rutaFisica,
                    FileMode.Create);

            await foto.CopyToAsync(
                stream);

            return
                $"/uploads/solicitudes/{solicitudId}/{nombreArchivo}";
        }

        // =========================================================
        // CALCULAR COTIZACIÓN
        // =========================================================

        private static void CalcularCotizacion(
            Cotizacion cotizacion)
        {
            cotizacion.Subtotal =
                cotizacion.CostoMateriales +
                cotizacion.ManoObra +
                cotizacion.CostoInstalacion +
                cotizacion.OtrosCostos;

            cotizacion.Impuesto = decimal.Round(cotizacion.Subtotal * cotizacion.PorcentajeImpuesto / 100m, 2, MidpointRounding.AwayFromZero);

            cotizacion.Total =
                cotizacion.Subtotal +
                cotizacion.Impuesto -
                cotizacion.Descuento;

            if (cotizacion.Total < 0)
            {
                cotizacion.Total = 0;
            }
        }
    }
}

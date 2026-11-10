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
    public class InstalacionesController : Controller
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


        // =====================================================
        // LISTADO DE INSTALACIONES
        // =====================================================

        public async Task<IActionResult> Index()
        {
            var trabajos =
                await _context
                    .TrabajosInstalacion

                    .AsNoTracking()

                    .Include(t =>
                        t.Instalador)

                    .Include(t =>
                        t.Compra)

                    .Include(t =>
                        t.SolicitudCotizacion)

                    .OrderBy(t =>
                        t.FechaInstalacion)

                    .ToListAsync();


            return View(trabajos);
        }


        // =====================================================
        // CREAR - GET
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> Create(
            int? compraId)
        {
            await CargarCombos(
                compraId,
                null);


            var modelo =
                new TrabajoInstalacion
                {
                    CompraId =
                        compraId,

                    FechaInstalacion =
                        DateTime.Now
                            .AddDays(3)
                            .Date
                            .AddHours(9)
                };


            return View(modelo);
        }


        // =====================================================
        // CREAR - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            TrabajoInstalacion trabajo)
        {
            /*
             * Cliente, Producto y Dirección NO son escritos
             * manualmente en el formulario.
             *
             * Se obtienen de la compra/solicitud.
             *
             * Por eso eliminamos su validación inicial del
             * ModelState antes de comprobar IsValid.
             */

            ModelState.Remove(
                nameof(
                    TrabajoInstalacion.Cliente));

            ModelState.Remove(
                nameof(
                    TrabajoInstalacion.Producto));

            ModelState.Remove(
                nameof(
                    TrabajoInstalacion.Direccion));


            // =================================================
            // VALIDAR COMPRA
            // =================================================

            if (!trabajo.CompraId.HasValue ||
                trabajo.CompraId.Value <= 0)
            {
                ModelState.AddModelError(
                    nameof(
                        trabajo.CompraId),

                    "Debe seleccionar una compra.");
            }


            // =================================================
            // VALIDAR INSTALADOR
            // =================================================

            if (string.IsNullOrWhiteSpace(
                trabajo.InstaladorId))
            {
                ModelState.AddModelError(
                    nameof(
                        trabajo.InstaladorId),

                    "Debe seleccionar un instalador.");
            }


            // =================================================
            // VALIDAR FECHA
            // =================================================

            if (trabajo.FechaInstalacion <=
                DateTime.Now)
            {
                ModelState.AddModelError(
                    nameof(
                        trabajo.FechaInstalacion),

                    "La fecha de instalación debe ser futura.");
            }


            // =================================================
            // BUSCAR COMPRA Y TODA SU INFORMACIÓN
            // =================================================

            Compra? compra = null;


            if (trabajo.CompraId.HasValue)
            {
                compra =
                    await _context
                        .Compras

                        .Include(c =>
                            c.Cliente)

                        .Include(c =>
                            c.Cotizacion)

                            .ThenInclude(c =>
                                c!.SolicitudCotizacion)

                                .ThenInclude(s =>
                                    s!.Producto)

                        .Include(c =>
                            c.Cotizacion)

                            .ThenInclude(c =>
                                c!.SolicitudCotizacion)

                                .ThenInclude(s =>
                                    s!.Material)

                        .Include(c =>
                            c.TrabajoInstalacion)

                        .FirstOrDefaultAsync(
                            c =>
                                c.Id ==
                                trabajo.CompraId.Value);
            }


            if (trabajo.CompraId.HasValue &&
                compra == null)
            {
                ModelState.AddModelError(
                    nameof(
                        trabajo.CompraId),

                    "La compra seleccionada no existe.");
            }


            // =================================================
            // VALIDAR ESTADO DE COMPRA
            // =================================================

            if (compra != null &&
                compra.Estado !=
                    "En producción")
            {
                ModelState.AddModelError(
                    nameof(
                        trabajo.CompraId),

                    "La compra debe tener el pago confirmado " +
                    "y estar en estado 'En producción'.");
            }


            // La compra solo es elegible si la fabricación terminó
            // y la solicitud requiere instalación.
            if (compra != null)
            {
                var solicitudValidacion = compra.Cotizacion?.SolicitudCotizacion;
                if (solicitudValidacion == null || !solicitudValidacion.RequiereInstalacion)
                    ModelState.AddModelError(nameof(trabajo.CompraId),
                        "La compra no corresponde a una solicitud que requiera instalación.");

                bool produccionTerminada = await _context.OrdenesProduccion
                    .AnyAsync(o => o.CompraId == compra.Id && o.Estado == "Terminada");
                if (!produccionTerminada)
                    ModelState.AddModelError(nameof(trabajo.CompraId),
                        "Primero debe terminarse la orden de producción.");
            }

            // =================================================
            // EVITAR DOBLE INSTALACIÓN
            // =================================================

            if (compra?.TrabajoInstalacion != null)
            {
                ModelState.AddModelError(
                    nameof(
                        trabajo.CompraId),

                    "Esta compra ya tiene una instalación asignada.");
            }


            // =================================================
            // VALIDAR QUE EL INSTALADOR EXISTA Y TENGA EL ROL
            // =================================================

            ApplicationUser? instalador =
                null;


            if (!string.IsNullOrWhiteSpace(
                trabajo.InstaladorId))
            {
                instalador =
                    await _userManager
                        .FindByIdAsync(
                            trabajo.InstaladorId);


                if (instalador == null ||
                    !instalador.Activo)
                {
                    ModelState.AddModelError(
                        nameof(
                            trabajo.InstaladorId),

                        "El instalador seleccionado no está disponible.");
                }
                else
                {
                    var esInstalador =
                        await _userManager
                            .IsInRoleAsync(
                                instalador,
                                "Instalador");


                    if (!esInstalador)
                    {
                        ModelState.AddModelError(
                            nameof(
                                trabajo.InstaladorId),

                            "El usuario seleccionado no tiene rol de Instalador.");
                    }
                }
            }


            // =================================================
            // SI HAY ERRORES
            // =================================================

            if (!ModelState.IsValid)
            {
                await CargarCombos(
                    trabajo.CompraId,
                    trabajo.InstaladorId);


                return View(trabajo);
            }


            // =================================================
            // OBTENER SOLICITUD
            // =================================================

            var solicitud =
                compra!
                    .Cotizacion?
                    .SolicitudCotizacion;


            if (solicitud == null)
            {
                ModelState.AddModelError(
                    string.Empty,

                    "No fue posible encontrar la solicitud " +
                    "relacionada con esta compra.");


                await CargarCombos(
                    trabajo.CompraId,
                    trabajo.InstaladorId);


                return View(trabajo);
            }


            // =================================================
            // COMPLETAR DATOS AUTOMÁTICAMENTE
            // =================================================

            trabajo.Cliente =
                compra.Cliente?
                    .NombreCompleto
                ??
                solicitud.NombreCliente;


            trabajo.Producto =
                solicitud.Producto?
                    .Nombre
                ??
                solicitud.TipoProducto;


            trabajo.Direccion =
                solicitud.Direccion
                ??
                compra.Cliente?
                    .Direccion
                ??
                "Dirección pendiente de confirmar";


            trabajo.SolicitudCotizacionId =
                solicitud.Id;


            trabajo.InstaladorId =
                instalador!.Id;


            trabajo.Estado =
                "Programada";


            trabajo.MedidasConfirmadas =
                false;

            trabajo.MaterialListo =
                false;

            trabajo.InstalacionIniciada =
                false;

            trabajo.InstalacionRealizada =
                false;

            trabajo.RevisionAcabados =
                false;

            trabajo.LimpiezaFinal =
                false;


            // =================================================
            // GUARDAR INSTALACIÓN
            // =================================================

            _context
                .TrabajosInstalacion
                .Add(trabajo);


            // Actualizar solicitud
            solicitud.Estado =
                "Instalación programada";


            await _context
                .SaveChangesAsync();


            TempData["Mensaje"] =
                $"Instalación asignada correctamente a " +
                $"{instalador.NombreCompleto}.";


            return RedirectToAction(
                nameof(Index));
        }


        // =====================================================
        // CARGAR COMPRAS E INSTALADORES
        // =====================================================

        // Reprogramación y reasignación administradas, sin modificar checklist.
        [HttpGet]
        public async Task<IActionResult> Reasignar(int id)
        {
            var trabajo = await _context.TrabajosInstalacion
                .Include(t => t.Instalador).FirstOrDefaultAsync(t => t.Id == id);
            if (trabajo == null) return NotFound();
            if (trabajo.Estado == "Finalizada" || trabajo.FechaFinalizacion != null)
            {
                TempData["Error"] = "Una instalación finalizada no puede reasignarse.";
                return RedirectToAction(nameof(Index));
            }
            await CargarInstaladores(trabajo.InstaladorId);
            return View(new GlassFlowAyF.Models.ViewModels.ReasignarInstalacionViewModel
            {
                Id = trabajo.Id,
                InstaladorId = trabajo.InstaladorId ?? "",
                FechaInstalacion = trabajo.FechaInstalacion,
                InstaladorActual = trabajo.Instalador?.NombreCompleto ?? "Sin asignar"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reasignar(GlassFlowAyF.Models.ViewModels.ReasignarInstalacionViewModel modelo)
        {
            var trabajo = await _context.TrabajosInstalacion
                .Include(t => t.Instalador).FirstOrDefaultAsync(t => t.Id == modelo.Id);
            if (trabajo == null) return NotFound();
            if (trabajo.Estado == "Finalizada" || trabajo.FechaFinalizacion != null
                || trabajo.InstalacionRealizada)
            {
                TempData["Error"] = "No se puede reasignar una instalación finalizada.";
                return RedirectToAction(nameof(Index));
            }
            if (modelo.FechaInstalacion <= DateTime.Now)
                ModelState.AddModelError(nameof(modelo.FechaInstalacion), "Seleccione una fecha futura.");
            var usuario = await _userManager.FindByIdAsync(modelo.InstaladorId ?? "");
            if (usuario == null || !usuario.Activo || !await _userManager.IsInRoleAsync(usuario, "Instalador"))
                ModelState.AddModelError(nameof(modelo.InstaladorId), "Seleccione un instalador activo con el rol correspondiente.");
            if (!ModelState.IsValid)
            {
                modelo.InstaladorActual = trabajo.Instalador?.NombreCompleto ?? "Sin asignar";
                await CargarInstaladores(modelo.InstaladorId);
                return View(modelo);
            }
            var anterior = trabajo.Instalador?.NombreCompleto ?? "Sin asignar";
            var fechaAnterior = trabajo.FechaInstalacion;
            trabajo.InstaladorId = usuario!.Id;
            trabajo.FechaInstalacion = modelo.FechaInstalacion;
            // Bitácora resumida en observaciones; para auditoría completa,
            // implementar posteriormente una tabla de historial propia.
            var nota = $"[{DateTime.Now:yyyy-MM-dd HH:mm}] Reasignación por {User.Identity?.Name}: " +
                $"{anterior} -> {usuario.NombreCompleto}; fecha {fechaAnterior:yyyy-MM-dd HH:mm} -> " +
                $"{modelo.FechaInstalacion:yyyy-MM-dd HH:mm}. Motivo: {modelo.Motivo.Trim()}";
            var previo = trabajo.Observaciones ?? "";
            trabajo.Observaciones = (previo + "\n" + nota).Trim();
            if (trabajo.Observaciones.Length > 1000)
            {
                ModelState.AddModelError("", "Las observaciones alcanzaron el límite. No se guardó la reasignación.");
                modelo.InstaladorActual = anterior;
                await CargarInstaladores(modelo.InstaladorId);
                return View(modelo);
            }
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Instalación reasignada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarInstaladores(string? seleccionado)
        {
            var usuarios = await _userManager.GetUsersInRoleAsync("Instalador");
            ViewBag.Instaladores = new SelectList(usuarios.Where(x => x.Activo)
                .OrderBy(x => x.NombreCompleto).ToList(), "Id", "NombreCompleto", seleccionado);
        }

        private async Task CargarCombos(
            int? compraId,
            string? instaladorId)
        {
            // =================================================
            // COMPRAS DISPONIBLES
            // =================================================

            var compras = await _context.Compras
                .AsNoTracking()
                .Include(c => c.Cliente)
                .Include(c => c.Cotizacion)
                    .ThenInclude(c => c!.SolicitudCotizacion)
                .Where(c => c.Estado == "En producción"
                    && c.TrabajoInstalacion == null
                    && c.Cotizacion != null
                    && c.Cotizacion.SolicitudCotizacion != null
                    && c.Cotizacion.SolicitudCotizacion.RequiereInstalacion
                    && _context.OrdenesProduccion.Any(o =>
                        o.CompraId == c.Id && o.Estado == "Terminada"))
                .OrderByDescending(c => c.FechaCompra)
                .Select(c => new {
                    c.Id,
                    Texto = c.NumeroOrden + " - " +
                        (c.Cliente != null ? c.Cliente.NombreCompleto : "Cliente")
                })
                .ToListAsync();

            ViewBag.Compras =
                new SelectList(
                    compras,
                    "Id",
                    "Texto",
                    compraId);


            // =================================================
            // INSTALADORES ACTIVOS
            // =================================================

            var instaladoresRol =
                await _userManager
                    .GetUsersInRoleAsync(
                        "Instalador");


            var instaladores =
                instaladoresRol

                    .Where(u =>
                        u.Activo)

                    .OrderBy(u =>
                        u.NombreCompleto)

                    .ToList();


            ViewBag.Instaladores =
                new SelectList(
                    instaladores,
                    "Id",
                    "NombreCompleto",
                    instaladorId);
        }
    }
}
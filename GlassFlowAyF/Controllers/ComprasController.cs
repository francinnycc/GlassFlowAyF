using ClosedXML.Excel;
using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers
{
    [Authorize]
    public class ComprasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ComprasController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // =========================================================
        // CLIENTE - MIS COMPRAS
        // =========================================================

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> MisCompras()
        {
            var usuario =
                await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return Unauthorized();
            }

            var compras =
                await _context.Compras

                    .AsNoTracking()

                    .Include(c => c.Factura)

                    .Include(c => c.Cotizacion)
                        .ThenInclude(c =>
                            c!.SolicitudCotizacion)
                        .ThenInclude(s =>
                            s!.Producto)

                    .Include(c => c.Cotizacion)
                        .ThenInclude(c =>
                            c!.SolicitudCotizacion)
                        .ThenInclude(s =>
                            s!.Material)

                    .Where(c =>
                        c.ClienteId == usuario.Id)

                    .OrderByDescending(c =>
                        c.FechaCompra)

                    .ToListAsync();

            return View(compras);
        }


        // =========================================================
        // CLIENTE - CONFIRMAR COMPRA GET
        // =========================================================

        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> Confirmar(
            int cotizacionId)
        {
            var usuario =
                await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return Unauthorized();
            }

            var cotizacion =
                await _context.Cotizaciones

                    .Include(c =>
                        c.SolicitudCotizacion)
                        .ThenInclude(s =>
                            s!.Producto)

                    .Include(c =>
                        c.SolicitudCotizacion)
                        .ThenInclude(s =>
                            s!.Material)

                    .Include(c =>
                        c.Compra)

                    .FirstOrDefaultAsync(c =>
                        c.Id == cotizacionId);

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

            if (cotizacion.Estado != "Aprobada")
            {
                TempData["Error"] =
                    "Primero debe aprobar la cotización.";

                return RedirectToAction(
                    "VerCotizacion",
                    "Cotizaciones",
                    new
                    {
                        id = cotizacionId
                    });
            }

            if (cotizacion.Compra != null)
            {
                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = cotizacion.Compra.Id
                    });
            }

            ViewBag.Cotizacion =
                cotizacion;

            return View();
        }


        // =========================================================
        // CLIENTE - CONFIRMAR COMPRA POST
        // =========================================================

        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(
            int cotizacionId,
            string metodoPago,
            string? observaciones)
        {
            var usuario =
                await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return Unauthorized();
            }

            var cotizacion =
                await _context.Cotizaciones

                    .Include(c =>
                        c.SolicitudCotizacion)
                        .ThenInclude(s =>
                            s!.Producto)

                    .Include(c =>
                        c.SolicitudCotizacion)
                        .ThenInclude(s =>
                            s!.Material)

                    .Include(c =>
                        c.Compra)

                    .FirstOrDefaultAsync(c =>
                        c.Id == cotizacionId);

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

            if (cotizacion.Estado != "Aprobada")
            {
                TempData["Error"] =
                    "La cotización debe estar aprobada antes de generar la compra.";

                return RedirectToAction(
                    "VerCotizacion",
                    "Cotizaciones",
                    new
                    {
                        id = cotizacionId
                    });
            }

            if (cotizacion.Compra != null)
            {
                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id = cotizacion.Compra.Id
                    });
            }


            // =====================================================
            // VALIDAR MÉTODO DE PAGO
            // =====================================================

            string[] metodosPermitidos =
            {
                "Transferencia",
                "SINPE",
                "SINPE Móvil",
                "Pago en oficina"
            };

            if (string.IsNullOrWhiteSpace(metodoPago) ||
                !metodosPermitidos.Contains(metodoPago))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Seleccione un método de pago válido.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Cotizacion =
                    cotizacion;

                return View();
            }


            // =====================================================
            // CREAR COMPRA
            // =====================================================

            var compra =
                new Compra
                {
                    CotizacionId =
                        cotizacion.Id,

                    ClienteId =
                        usuario.Id,

                    NumeroOrden =
                        $"GF-ORD-{DateTime.Now:yyyy}-{Guid.NewGuid()
                            .ToString("N")[..6]
                            .ToUpperInvariant()}",

                    MetodoPago =
                        metodoPago,

                    Estado =
                        "Pendiente de pago",

                    Total =
                        cotizacion.Total,

                    Observaciones =
                        string.IsNullOrWhiteSpace(observaciones)
                            ? null
                            : observaciones.Trim(),

                    FechaCompra =
                        DateTime.Now
                };

            _context.Compras.Add(compra);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Compra registrada correctamente. " +
                "La orden queda pendiente de confirmación de pago.";

            return RedirectToAction(
                nameof(Details),
                new
                {
                    id = compra.Id
                });
        }


        // =========================================================
        // DETALLE DE COMPRA
        // CLIENTE O ADMINISTRADOR
        // =========================================================

        [Authorize(Roles = "Cliente,Administrador")]
        public async Task<IActionResult> Details(
            int id)
        {
            var compra =
                await _context.Compras

                    .AsNoTracking()

                    .Include(c =>
                        c.Cliente)

                    .Include(c =>
                        c.Factura)

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

                    .FirstOrDefaultAsync(c =>
                        c.Id == id);

            if (compra == null)
            {
                return NotFound();
            }


            // =====================================================
            // SEGURIDAD DEL CLIENTE
            // =====================================================

            if (User.IsInRole("Cliente"))
            {
                var usuario =
                    await _userManager.GetUserAsync(User);

                if (usuario == null ||
                    compra.ClienteId != usuario.Id)
                {
                    return Forbid();
                }
            }

            return View(compra);
        }


        // =========================================================
        // ADMINISTRADOR - TODAS LAS COMPRAS
        // =========================================================

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Administrar()
        {
            var compras =
                await _context.Compras

                    .AsNoTracking()

                    .Include(c =>
                        c.Cliente)

                    .Include(c =>
                        c.Factura)

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

                    .OrderByDescending(c =>
                        c.FechaCompra)

                    .ToListAsync();

            return View(compras);
        }


        // =========================================================
        // ADMINISTRADOR - CONFIRMAR PAGO
        // =========================================================

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarPagada(
            int id)
        {
            var compra =
                await _context.Compras

                    .Include(c =>
                        c.Factura)

                    .Include(c =>
                        c.Cotizacion)
                        .ThenInclude(c =>
                            c!.SolicitudCotizacion)

                    .FirstOrDefaultAsync(c =>
                        c.Id == id);

            if (compra == null)
            {
                return NotFound();
            }


            // No permitir regresar una compra que ya avanzó
            // nuevamente a "En producción".
            if (compra.Estado != "Pendiente de pago")
            {
                TempData["Error"] =
                    "Esta compra ya fue procesada.";

                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id
                    });
            }

            if (compra.Cotizacion == null)
            {
                TempData["Error"] =
                    "La compra no posee una cotización asociada.";

                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id
                    });
            }


            compra.Estado =
                "En producción";

            compra.FechaPago =
                DateTime.Now;


            // =====================================================
            // GENERAR FACTURA
            // =====================================================

            if (compra.Factura == null)
            {
                var factura =
                    new Factura
                    {
                        CompraId =
                            compra.Id,

                        NumeroFactura =
                            $"FAC-GF-{DateTime.Now:yyyy}-{compra.Id:D5}",

                        FechaEmision =
                            DateTime.Now,

                        Subtotal =
                            compra.Cotizacion.Subtotal,

                        Impuesto =
                            compra.Cotizacion.Impuesto,

                        Descuento =
                            compra.Cotizacion.Descuento,

                        Total =
                            compra.Cotizacion.Total,

                        Estado =
                            "Emitida"
                    };

                _context.Facturas.Add(
                    factura);
            }


            // =====================================================
            // ACTUALIZAR SOLICITUD
            // =====================================================

            if (compra
                    .Cotizacion
                    .SolicitudCotizacion != null)
            {
                compra
                    .Cotizacion
                    .SolicitudCotizacion
                    .Estado =
                        "En producción";
            }

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Pago confirmado y factura generada correctamente.";

            return RedirectToAction(
                nameof(Details),
                new
                {
                    id
                });
        }


        // =========================================================
        // EXPORTAR FACTURA A EXCEL
        // =========================================================

        [Authorize(Roles = "Cliente,Administrador")]
        public async Task<IActionResult> ExportarFacturaExcel(
            int id)
        {
            var factura =
                await _context.Facturas

                    .AsNoTracking()

                    .Include(f =>
                        f.Compra)
                        .ThenInclude(c =>
                            c!.Cliente)

                    .Include(f =>
                        f.Compra)
                        .ThenInclude(c =>
                            c!.Cotizacion)
                        .ThenInclude(c =>
                            c!.SolicitudCotizacion)
                        .ThenInclude(s =>
                            s!.Producto)

                    .Include(f =>
                        f.Compra)
                        .ThenInclude(c =>
                            c!.Cotizacion)
                        .ThenInclude(c =>
                            c!.SolicitudCotizacion)
                        .ThenInclude(s =>
                            s!.Material)

                    .FirstOrDefaultAsync(f =>
                        f.Id == id);

            if (factura == null)
            {
                return NotFound();
            }


            // =====================================================
            // SEGURIDAD DEL CLIENTE
            // =====================================================

            if (User.IsInRole("Cliente"))
            {
                var usuario =
                    await _userManager.GetUserAsync(User);

                if (usuario == null ||
                    factura.Compra?.ClienteId != usuario.Id)
                {
                    return Forbid();
                }
            }


            var compra =
                factura.Compra;

            var cotizacion =
                compra?.Cotizacion;

            var solicitud =
                cotizacion?.SolicitudCotizacion;


            // =====================================================
            // CREAR EXCEL
            // =====================================================

            using var workbook =
                new XLWorkbook();

            var hoja =
                workbook.Worksheets.Add(
                    "Factura");


            // =====================================================
            // ENCABEZADO
            // =====================================================

            hoja.Cell("A1").Value =
                "GLASSFLOW A&F";

            hoja.Cell("A2").Value =
                "FACTURA";

            hoja.Range("A1:B1")
                .Merge();

            hoja.Range("A2:B2")
                .Merge();

            hoja.Cell("A1")
                .Style.Font.Bold = true;

            hoja.Cell("A1")
                .Style.Font.FontSize = 20;

            hoja.Cell("A2")
                .Style.Font.Bold = true;

            hoja.Cell("A2")
                .Style.Font.FontSize = 14;


            // =====================================================
            // DATOS DE FACTURA
            // =====================================================

            hoja.Cell("A4").Value =
                "Factura No.";

            hoja.Cell("B4").Value =
                factura.NumeroFactura;

            hoja.Cell("A5").Value =
                "Fecha de emisión";

            hoja.Cell("B5").Value =
                factura.FechaEmision;

            hoja.Cell("B5")
                .Style.DateFormat
                .Format =
                    "dd/MM/yyyy hh:mm";


            // =====================================================
            // CLIENTE
            // =====================================================

            hoja.Cell("A7").Value =
                "Cliente";

            hoja.Cell("B7").Value =
                compra?
                    .Cliente?
                    .NombreCompleto
                ?? solicitud?
                    .NombreCliente
                ?? "";

            hoja.Cell("A8").Value =
                "Correo";

            hoja.Cell("B8").Value =
                compra?
                    .Cliente?
                    .Email
                ?? solicitud?
                    .Correo
                ?? "";

            hoja.Cell("A9").Value =
                "Orden";

            hoja.Cell("B9").Value =
                compra?
                    .NumeroOrden
                ?? "";


            // =====================================================
            // PROYECTO
            // =====================================================

            hoja.Cell("A11").Value =
                "Producto";

            hoja.Cell("B11").Value =
                solicitud?
                    .Producto?
                    .Nombre
                ?? solicitud?
                    .TipoProducto
                ?? "";

            hoja.Cell("A12").Value =
                "Material";

            hoja.Cell("B12").Value =
                solicitud?
                    .Material?
                    .Nombre
                ?? "No especificado";


            // =====================================================
            // TOTALES
            // =====================================================

            hoja.Cell("A14").Value =
                "Subtotal";

            hoja.Cell("B14").Value =
                factura.Subtotal;

            hoja.Cell("A15").Value =
                "Impuesto";

            hoja.Cell("B15").Value =
                factura.Impuesto;

            hoja.Cell("A16").Value =
                "Descuento";

            hoja.Cell("B16").Value =
                factura.Descuento;

            hoja.Cell("A17").Value =
                "TOTAL";

            hoja.Cell("B17").Value =
                factura.Total;


            hoja.Range("A17:B17")
                .Style.Font.Bold = true;

            hoja.Range("A17:B17")
                .Style.Font.FontSize = 13;


            hoja.Range("B14:B17")
                .Style.NumberFormat
                .Format =
                    "₡#,##0.00";


            // =====================================================
            // ESTILO GENERAL
            // =====================================================

            hoja.Column("A")
                .Width =
                    24;

            hoja.Column("B")
                .Width =
                    45;

            hoja.Range("A4:A17")
                .Style.Font.Bold = true;

            hoja.Columns()
                .AdjustToContents();


            // =====================================================
            // DESCARGAR
            // =====================================================

            using var stream =
                new MemoryStream();

            workbook.SaveAs(
                stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{factura.NumeroFactura}.xlsx");
        }
    }
}
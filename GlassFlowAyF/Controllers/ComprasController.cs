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

        private readonly UserManager<ApplicationUser>
            _userManager;


        public ComprasController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult>
            MisCompras()
        {
            var usuario =
                await _userManager.GetUserAsync(
                    User);

            if (usuario == null)
            {
                return Unauthorized();
            }

            var compras =
                await _context.Compras
                    .Include(c => c.Cotizacion)
                        .ThenInclude(c =>
                            c!.SolicitudCotizacion)
                            .ThenInclude(s =>
                                s!.Producto)
                    .Include(c => c.Factura)
                    .Where(c =>
                        c.ClienteId ==
                        usuario.Id)
                    .OrderByDescending(
                        c => c.FechaCompra)
                    .ToListAsync();

            return View(compras);
        }


        [Authorize(Roles = "Cliente")]
        [HttpGet]
        public async Task<IActionResult> Confirmar(
            int cotizacionId)
        {
            var usuario =
                await _userManager.GetUserAsync(
                    User);

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

         .FirstOrDefaultAsync(
             c =>
                 c.Id ==
                 cotizacionId);

            if (cotizacion == null)
            {
                return NotFound();
            }

            if (usuario == null ||
                cotizacion
                    .SolicitudCotizacion?
                    .Correo != usuario.Email)
            {
                return Forbid();
            }

            if (cotizacion.Estado !=
                "Aprobada")
            {
                TempData["Error"] =
                    "Primero debe aprobar la cotización.";

                return RedirectToAction(
                    "VerCotizacion",
                    "Cotizaciones",
                    new { id = cotizacionId });
            }

            if (cotizacion.Compra != null)
            {
                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id =
                            cotizacion
                                .Compra.Id
                    });
            }

            ViewBag.Cotizacion =
                cotizacion;

            return View();
        }


        [Authorize(Roles = "Cliente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(
            int cotizacionId,
            string metodoPago,
            string? observaciones)
        {
            var usuario =
                await _userManager.GetUserAsync(
                    User);

            if (usuario == null)
            {
                return Unauthorized();
            }

            var cotizacion =
                await _context.Cotizaciones
                    .Include(c =>
                        c.SolicitudCotizacion)
                    .Include(c => c.Compra)
                    .FirstOrDefaultAsync(
                        c => c.Id ==
                            cotizacionId);

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

            if (cotizacion.Estado !=
                "Aprobada")
            {
                return BadRequest();
            }

            if (cotizacion.Compra != null)
            {
                return RedirectToAction(
                    nameof(Details),
                    new
                    {
                        id =
                            cotizacion
                                .Compra.Id
                    });
            }


            string[] metodosPermitidos =
            {
                "Transferencia",
                "SINPE",
                "Pago en oficina"
            };

            if (!metodosPermitidos
                .Contains(metodoPago))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Seleccione un método de pago válido.");

                ViewBag.Cotizacion =
                    cotizacion;

                return View();
            }


            var compra =
                new Compra
                {
                    CotizacionId =
                        cotizacion.Id,

                    ClienteId =
                        usuario.Id,

                    NumeroOrden =
                        $"ORD-{DateTime.Now:yyyy}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",

                    MetodoPago =
                        metodoPago,

                    Estado =
                        "Pendiente de pago",

                    Total =
                        cotizacion.Total,

                    Observaciones =
                        observaciones,

                    FechaCompra =
                        DateTime.Now
                };


            _context.Compras.Add(
                compra);

            await _context.SaveChangesAsync();


            TempData["Mensaje"] =
                "Compra registrada. La orden está pendiente de confirmación de pago.";


            return RedirectToAction(
                nameof(Details),
                new { id = compra.Id });
        }


        [Authorize]
        public async Task<IActionResult> Details(
            int id)
        {
            var compra =
                await _context.Compras
                    .Include(c => c.Cliente)
                    .Include(c => c.Factura)
                    .Include(c => c.Cotizacion)
                        .ThenInclude(c =>
                            c!.SolicitudCotizacion)
                            .ThenInclude(s =>
                                s!.Producto)
                    .FirstOrDefaultAsync(
                        c => c.Id == id);

            if (compra == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Cliente"))
            {
                var usuario =
                    await _userManager
                        .GetUserAsync(User);

                if (usuario == null ||
                    compra.ClienteId !=
                    usuario.Id)
                {
                    return Forbid();
                }
            }

            return View(compra);
        }


        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult>
            Administrar()
        {
            var compras =
                await _context.Compras
                    .Include(c => c.Cliente)
                    .Include(c => c.Factura)
                    .Include(c => c.Cotizacion)
                        .ThenInclude(c =>
                            c!.SolicitudCotizacion)
                    .OrderByDescending(
                        c => c.FechaCompra)
                    .ToListAsync();

            return View(compras);
        }


        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            MarcarPagada(
                int id)
        {
            var compra =
                await _context.Compras
                    .Include(c => c.Factura)
                    .Include(c => c.Cotizacion)
                        .ThenInclude(c =>
                            c!.SolicitudCotizacion)
                    .FirstOrDefaultAsync(
                        c => c.Id == id);

            if (compra == null)
            {
                return NotFound();
            }


            if (compra.Estado == "Pagada" ||
                compra.Estado ==
                "En producción")
            {
                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }


            compra.Estado =
                "En producción";

            compra.FechaPago =
                DateTime.Now;


            var cotizacion =
                compra.Cotizacion!;


            if (compra.Factura == null)
            {
                var factura =
                    new Factura
                    {
                        CompraId =
                            compra.Id,

                        NumeroFactura =
                            $"FAC-{DateTime.Now:yyyy}-{compra.Id:D5}",

                        FechaEmision =
                            DateTime.Now,

                        Subtotal =
                            cotizacion.Subtotal,

                        Impuesto =
                            cotizacion.Impuesto,

                        Descuento =
                            cotizacion.Descuento,

                        Total =
                            cotizacion.Total,

                        Estado =
                            "Emitida"
                    };


                _context.Facturas.Add(
                    factura);
            }


            if (cotizacion
                .SolicitudCotizacion != null)
            {
                cotizacion
                    .SolicitudCotizacion
                    .Estado =
                        "En producción";
            }


            await _context
                .SaveChangesAsync();


            TempData["Mensaje"] =
                "Pago confirmado y factura generada correctamente.";


            return RedirectToAction(
                nameof(Details),
                new { id });
        }


        [Authorize]
        public async Task<IActionResult>
            ExportarFacturaExcel(
                int id)
        {
            var factura =
                await _context.Facturas
                    .Include(f => f.Compra)
                        .ThenInclude(c =>
                            c!.Cliente)
                    .Include(f => f.Compra)
                        .ThenInclude(c =>
                            c!.Cotizacion)
                            .ThenInclude(c =>
                                c!.SolicitudCotizacion)
                                .ThenInclude(s =>
                                    s!.Producto)
                    .FirstOrDefaultAsync(
                        f => f.Id == id);


            if (factura == null)
            {
                return NotFound();
            }


            if (User.IsInRole("Cliente"))
            {
                var usuario =
                    await _userManager
                        .GetUserAsync(User);

                if (usuario == null ||
                    factura.Compra?.ClienteId !=
                    usuario.Id)
                {
                    return Forbid();
                }
            }


            using var workbook =
                new XLWorkbook();

            var hoja =
                workbook.Worksheets.Add(
                    "Factura");


            hoja.Cell("A1").Value =
                "GLASSFLOW A&F";

            hoja.Cell("A2").Value =
                "FACTURA";


            hoja.Cell("A4").Value =
                "Factura No.";

            hoja.Cell("B4").Value =
                factura.NumeroFactura;


            hoja.Cell("A5").Value =
                "Fecha";

            hoja.Cell("B5").Value =
                factura.FechaEmision;


            hoja.Cell("A7").Value =
                "Cliente";

            hoja.Cell("B7").Value =
                factura.Compra?
                    .Cliente?
                    .NombreCompleto ?? "";


            hoja.Cell("A8").Value =
                "Correo";

            hoja.Cell("B8").Value =
                factura.Compra?
                    .Cliente?
                    .Email ?? "";


            hoja.Cell("A9").Value =
                "Orden";

            hoja.Cell("B9").Value =
                factura.Compra?
                    .NumeroOrden ?? "";


            hoja.Cell("A11").Value =
                "Producto";

            hoja.Cell("B11").Value =
                factura.Compra?
                    .Cotizacion?
                    .SolicitudCotizacion?
                    .Producto?
                    .Nombre
                ??
                factura.Compra?
                    .Cotizacion?
                    .SolicitudCotizacion?
                    .TipoProducto
                ?? "";


            hoja.Cell("A13").Value =
                "Subtotal";

            hoja.Cell("B13").Value =
                factura.Subtotal;


            hoja.Cell("A14").Value =
                "Impuesto";

            hoja.Cell("B14").Value =
                factura.Impuesto;


            hoja.Cell("A15").Value =
                "Descuento";

            hoja.Cell("B15").Value =
                factura.Descuento;


            hoja.Cell("A16").Value =
                "TOTAL";

            hoja.Cell("B16").Value =
                factura.Total;


            hoja.Range("A1:B1")
                .Merge();

            hoja.Cell("A1")
                .Style.Font.Bold = true;

            hoja.Cell("A1")
                .Style.Font.FontSize = 18;

            hoja.Range("A16:B16")
                .Style.Font.Bold = true;

            hoja.Range("B13:B16")
                .Style.NumberFormat
                .Format =
                    "₡#,##0.00";

            hoja.Columns()
                .AdjustToContents();


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
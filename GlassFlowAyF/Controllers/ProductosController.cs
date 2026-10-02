using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductosController(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // =========================================================
        // CATÁLOGO PÚBLICO
        // =========================================================

        [AllowAnonymous]
        public async Task<IActionResult> Index(
            string? categoria,
            string? buscar)
        {
            var consulta = _context.Productos
                .AsNoTracking()
                .Where(p => p.Activo);

            // FILTRO POR CATEGORÍA
            if (!string.IsNullOrWhiteSpace(categoria))
            {
                consulta = consulta.Where(
                    p => p.Categoria == categoria);
            }

            // BÚSQUEDA POR VARIAS PALABRAS
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var terminos = buscar
                    .Trim()
                    .Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries |
                        StringSplitOptions.TrimEntries);

                foreach (var termino in terminos)
                {
                    var palabra = termino;

                    consulta = consulta.Where(p =>
                        p.Nombre.Contains(palabra) ||
                        p.Categoria.Contains(palabra) ||
                        (
                            p.Descripcion != null &&
                            p.Descripcion.Contains(palabra)
                        ));
                }
            }

            // CATEGORÍAS REALES DE LOS PRODUCTOS ACTIVOS
            ViewBag.Categorias = await _context.Productos
                .Where(p => p.Activo)
                .Select(p => p.Categoria)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            ViewBag.Categoria = categoria;
            ViewBag.Buscar = buscar;

            var productos = await consulta
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            return View(productos);
        }

        // =========================================================
        // DETALLE DEL PRODUCTO
        // =========================================================

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var producto = await _context.Productos
                .AsNoTracking()
                .Include(p => p.ProductoMateriales)
                .ThenInclude(pm => pm.Material)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // =========================================================
        // ADMINISTRACIÓN DE PRODUCTOS
        // =========================================================

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Administrar()
        {
            var productos = await _context.Productos
                .OrderBy(p => p.Categoria)
                .ThenBy(p => p.Nombre)
                .ToListAsync();

            return View(productos);
        }

        // =========================================================
        // CREAR PRODUCTO - GET
        // =========================================================

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Materiales = await _context.Materiales
                .Where(m => m.Activo)
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            return View(new Producto());
        }

        // =========================================================
        // CREAR PRODUCTO - POST
        // =========================================================

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Producto producto,
            IFormFile? imagen,
            List<int>? materialIds)
        {
            if (imagen != null && !ImagenValida(imagen))
            {
                ModelState.AddModelError(
                    "imagen",
                    "La imagen debe ser JPG, JPEG, PNG o WEBP y pesar máximo 5 MB.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Materiales = await _context.Materiales
                    .Where(m => m.Activo)
                    .OrderBy(m => m.Nombre)
                    .ToListAsync();

                ViewBag.MaterialIds =
                    materialIds ?? new List<int>();

                return View(producto);
            }

            producto.FechaCreacion = DateTime.Now;
            producto.Activo = true;

            if (imagen != null)
            {
                producto.ImagenUrl =
                    await GuardarImagenProducto(imagen);
            }

            _context.Productos.Add(producto);

            await _context.SaveChangesAsync();

            if (materialIds != null)
            {
                foreach (var materialId in materialIds.Distinct())
                {
                    _context.ProductoMateriales.Add(
                        new ProductoMaterial
                        {
                            ProductoId = producto.Id,
                            MaterialId = materialId
                        });
                }

                await _context.SaveChangesAsync();
            }

            TempData["Mensaje"] =
                "Producto creado correctamente.";

            return RedirectToAction(nameof(Administrar));
        }

        // =========================================================
        // EDITAR PRODUCTO - GET
        // =========================================================

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.ProductoMateriales)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            ViewBag.Materiales = await _context.Materiales
                .Where(m => m.Activo)
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            ViewBag.MaterialIds = producto.ProductoMateriales
                .Select(pm => pm.MaterialId)
                .ToList();

            return View(producto);
        }

        // =========================================================
        // EDITAR PRODUCTO - POST
        // =========================================================

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Producto producto,
            IFormFile? imagen,
            List<int>? materialIds)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            var productoActual = await _context.Productos
                .Include(p => p.ProductoMateriales)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productoActual == null)
            {
                return NotFound();
            }

            if (imagen != null && !ImagenValida(imagen))
            {
                ModelState.AddModelError(
                    "imagen",
                    "La imagen debe ser JPG, JPEG, PNG o WEBP y pesar máximo 5 MB.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Materiales = await _context.Materiales
                    .Where(m => m.Activo)
                    .OrderBy(m => m.Nombre)
                    .ToListAsync();

                ViewBag.MaterialIds =
                    materialIds ?? new List<int>();

                return View(producto);
            }

            productoActual.Nombre = producto.Nombre;
            productoActual.Categoria = producto.Categoria;
            productoActual.Descripcion = producto.Descripcion;
            productoActual.PrecioBase = producto.PrecioBase;
            productoActual.PermiteInstalacion =
                producto.PermiteInstalacion;

            if (imagen != null)
            {
                if (!string.IsNullOrWhiteSpace(
                    productoActual.ImagenUrl))
                {
                    EliminarImagenAnterior(
                        productoActual.ImagenUrl);
                }

                productoActual.ImagenUrl =
                    await GuardarImagenProducto(imagen);
            }

            _context.ProductoMateriales.RemoveRange(
                productoActual.ProductoMateriales);

            if (materialIds != null)
            {
                foreach (var materialId in materialIds.Distinct())
                {
                    productoActual.ProductoMateriales.Add(
                        new ProductoMaterial
                        {
                            ProductoId = productoActual.Id,
                            MaterialId = materialId
                        });
                }
            }

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Producto actualizado correctamente.";

            return RedirectToAction(nameof(Administrar));
        }

        // =========================================================
        // ACTIVAR / DESACTIVAR PRODUCTO
        // =========================================================

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var producto =
                await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            producto.Activo = !producto.Activo;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                producto.Activo
                    ? "Producto activado correctamente."
                    : "Producto desactivado correctamente.";

            return RedirectToAction(nameof(Administrar));
        }

        // =========================================================
        // MATERIALES SEGÚN PRODUCTO
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> MaterialesPorProducto(
            int productoId)
        {
            var materiales = await _context.ProductoMateriales
                .Where(pm =>
                    pm.ProductoId == productoId &&
                    pm.Material != null &&
                    pm.Material.Activo)
                .Select(pm => new
                {
                    id = pm.MaterialId,

                    nombre =
                        pm.Material!.Nombre,

                    descripcion =
                        $"{pm.Material.Tipo} - " +
                        $"{pm.Material.Color} - " +
                        $"{pm.Material.Grosor} mm",

                    precio =
                        pm.Material.PrecioAdicional
                })
                .ToListAsync();

            return Json(materiales);
        }

        // =========================================================
        // VALIDAR IMAGEN
        // =========================================================

        private bool ImagenValida(IFormFile imagen)
        {
            var extensiones = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            var extension = Path
                .GetExtension(imagen.FileName)
                .ToLowerInvariant();

            return extensiones.Contains(extension) &&
                   imagen.Length <= 5 * 1024 * 1024;
        }

        // =========================================================
        // GUARDAR IMAGEN
        // =========================================================

        private async Task<string> GuardarImagenProducto(
            IFormFile imagen)
        {
            var extension = Path
                .GetExtension(imagen.FileName)
                .ToLowerInvariant();

            var carpeta = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "productos");

            Directory.CreateDirectory(carpeta);

            var nombreArchivo =
                $"{Guid.NewGuid()}{extension}";

            var rutaFisica = Path.Combine(
                carpeta,
                nombreArchivo);

            await using var stream = new FileStream(
                rutaFisica,
                FileMode.Create);

            await imagen.CopyToAsync(stream);

            return $"/uploads/productos/{nombreArchivo}";
        }

        // =========================================================
        // ELIMINAR IMAGEN ANTERIOR
        // =========================================================

        private void EliminarImagenAnterior(string imagenUrl)
        {
            var ruta = imagenUrl
                .TrimStart('/')
                .Replace(
                    '/',
                    Path.DirectorySeparatorChar);

            var rutaFisica = Path.Combine(
                _environment.WebRootPath,
                ruta);

            if (System.IO.File.Exists(rutaFisica))
            {
                System.IO.File.Delete(rutaFisica);
            }
        }
    }
}
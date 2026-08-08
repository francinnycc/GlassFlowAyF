using GlassFlowAyF.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? categoria)
        {
            var consulta = _context.Productos
                .AsNoTracking()
                .Where(p => p.Activo);

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                consulta = consulta.Where(
                    p => p.Categoria == categoria);
            }

            ViewBag.Categorias =
                await _context.Productos
                    .Where(p => p.Activo)
                    .Select(p => p.Categoria)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();

            return View(
                await consulta
                    .OrderBy(p => p.Nombre)
                    .ToListAsync());
        }
    }
}
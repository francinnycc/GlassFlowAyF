using GlassFlowAyF.Data;
using GlassFlowAyF.ViewModels;
using GlassFlowAyF.Extensions;
using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class MaterialesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaterialesController(
            ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index([FromQuery] FiltroViewModel filtro)
        {
            var consulta = _context.Materiales.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.Buscar))
            {
                var texto = filtro.Buscar.Trim();

                consulta = consulta.Where(m =>
                    m.Nombre.Contains(texto) ||
                    m.Tipo.Contains(texto) ||
                    (m.Color != null && m.Color.Contains(texto)) ||
                    (m.Acabado != null && m.Acabado.Contains(texto)));
            }

            if (filtro.Estado == "Activo" || filtro.Estado == "Inactivo")
            {
                var activo = filtro.Estado == "Activo";
                consulta = consulta.Where(m => m.Activo == activo);
            }

            var resultado = await consulta
                .OrderBy(m => m.Tipo)
                .ThenBy(m => m.Nombre)
                .PaginarAsync(filtro.Pagina, filtro.TamanoPagina);

            filtro.Estados = new List<string> { "Activo", "Inactivo" };
            filtro.MostrarFechas = false;
            filtro.Placeholder = "Nombre, tipo, color o acabado";

            ViewBag.Filtro = filtro;
            ViewBag.Paginacion = resultado.Paginacion;

            return View(resultado);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(new Material());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Material material)
        {
            if (!ModelState.IsValid)
            {
                return View(material);
            }

            material.Activo = true;

            _context.Materiales.Add(material);

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Material creado correctamente.";

            return RedirectToAction(
                nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(
            int id)
        {
            var material =
                await _context.Materiales
                    .FindAsync(id);

            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Material material)
        {
            if (id != material.Id)
            {
                return NotFound();
            }

            var actual =
                await _context.Materiales
                    .FindAsync(id);

            if (actual == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(material);
            }

            actual.Nombre = material.Nombre;
            actual.Tipo = material.Tipo;
            actual.Color = material.Color;
            actual.Perfil = material.Perfil;
            actual.Acabado = material.Acabado;
            actual.Grosor = material.Grosor;
            actual.PrecioAdicional =
                material.PrecioAdicional;
            actual.Descripcion =
                material.Descripcion;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                "Material actualizado correctamente.";

            return RedirectToAction(
                nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(
            int id)
        {
            var material =
                await _context.Materiales
                    .FindAsync(id);

            if (material == null)
            {
                return NotFound();
            }

            material.Activo = !material.Activo;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] =
                material.Activo
                    ? "Material activado."
                    : "Material desactivado.";

            return RedirectToAction(
                nameof(Index));
        }
    }
}
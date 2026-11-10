
using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers
{
    [Authorize(Roles = "Tecnico")]
    public class TecnicoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TecnicoController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return Forbid();
            }

            var visitas = await _context.VisitasTecnicas
                .AsNoTracking()
                .Include(v => v.SolicitudCotizacion)
                .Where(v => v.TecnicoAsignado == usuario.NombreCompleto)
                .OrderBy(v => v.FechaHora)
                .ToListAsync();

            return View(visitas);
        }
    }
}
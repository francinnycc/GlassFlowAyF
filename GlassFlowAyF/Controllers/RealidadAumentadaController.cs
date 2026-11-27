using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using GlassFlowAyF.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace GlassFlowAyF.Controllers;

[Authorize(Roles = "Administrador,Cliente")]
public class RealidadAumentadaController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<ApplicationUser> _users;
    public RealidadAumentadaController(ApplicationDbContext db, UserManager<ApplicationUser> users, IWebHostEnvironment env) { _db = db; _users = users; _env = env; }
    private async Task<IQueryable<SolicitudCotizacion>> Permitidas()
    {
        var q = _db.SolicitudesCotizacion.AsNoTracking();
        if (User.IsInRole("Administrador")) return q;
        var email = (await _users.GetUserAsync(User))?.Email;
        return string.IsNullOrWhiteSpace(email) ? q.Where(x => false) : q.Where(x => x.Correo == email);
    }
    private IQueryable<Material> Materiales(SolicitudCotizacion s) => _db.Materiales.AsNoTracking().Where(m => m.Activo && (s.ProductoId == null ? m.Id == s.MaterialId : _db.ProductoMateriales.Any(pm => pm.ProductoId == s.ProductoId && pm.MaterialId == m.Id)));
    public async Task<IActionResult> Index() => View(await (await Permitidas()).OrderByDescending(x => x.Id).ToListAsync());
    public async Task<IActionResult> Detalle(int id)
    {
        var s = await (await Permitidas()).FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return NotFound();
        return View(new RealidadAumentadaViewModel { Solicitud = s, Materiales = await Materiales(s).OrderBy(x => x.Nombre).ToListAsync(), Historial = await _db.ConfiguracionesAR.AsNoTracking().Include(x => x.Material).Where(x => x.SolicitudCotizacionId == id).OrderByDescending(x => x.Id).ToListAsync() });
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(int id, int materialId, string forma, string acabado, string vidrio, bool usoAR)
    {
        var s = await (await Permitidas()).FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return NotFound();
        if (!new[] { "baranda", "mampara", "puerta", "division" }.Contains(forma) || !new[] { "negro", "aluminio", "blanco" }.Contains(acabado) || !new[] { "claro", "esmerilado" }.Contains(vidrio) || !await Materiales(s).AnyAsync(x => x.Id == materialId)) return BadRequest("Configuración inválida.");
        _db.ConfiguracionesAR.Add(new ConfiguracionAR { SolicitudCotizacionId = id, MaterialId = materialId, Forma = forma, Acabado = acabado, Vidrio = vidrio, UsoAR = usoAR, UsuarioId = _users.GetUserId(User) ?? "" });
        await _db.SaveChangesAsync();
        TempData["ARGuardado"] = "Configuración guardada como referencia para cotización. No modifica precios ni medidas.";
        return RedirectToAction(nameof(Detalle), new { id });
    }
    [HttpGet]
    public IActionResult Modelo(string forma, string acabado, string vidrio)
    {
        if (!new[] { "baranda", "mampara", "puerta", "division" }.Contains(forma) || !new[] { "negro", "aluminio", "blanco" }.Contains(acabado) || !new[] { "claro", "esmerilado" }.Contains(vidrio)) return NotFound();
        return PhysicalFile(System.IO.Path.Combine(_env.WebRootPath, "ar", $"{forma}-{acabado}-{vidrio}.glb"), "model/gltf-binary");
    }
}

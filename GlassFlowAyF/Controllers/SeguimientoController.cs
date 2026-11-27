using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using GlassFlowAyF.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace GlassFlowAyF.Controllers;

[Authorize(Roles = "Administrador,Cliente")]
public class SeguimientoController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    public SeguimientoController(ApplicationDbContext db, UserManager<ApplicationUser> users) { _db = db; _users = users; }
    private async Task<IQueryable<SolicitudCotizacion>> Permitidas()
    {
        var q = _db.SolicitudesCotizacion.AsNoTracking();
        if (User.IsInRole("Administrador")) return q;
        var email = (await _users.GetUserAsync(User))?.Email;
        return string.IsNullOrWhiteSpace(email) ? q.Where(x => false) : q.Where(x => x.Correo == email);
    }
    public async Task<IActionResult> Index(string? buscar)
    {
        var q = await Permitidas();
        if (!string.IsNullOrWhiteSpace(buscar)) q = q.Where(x => x.NombreCliente.Contains(buscar) || x.TipoProducto.Contains(buscar));
        return View(await q.OrderByDescending(x => x.Id).ToListAsync());
    }
    public async Task<IActionResult> Detalle(int id)
    {
        var s = await (await Permitidas()).FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return NotFound();
        var m = new SeguimientoProyectoViewModel { Solicitud = s };
        var keys = new Dictionary<string, HashSet<int>>();
        void Add(string entidad, int registro, string etapa, string referencia, string estado, DateTime? fecha, string? alerta = null)
        {
            m.Etapas.Add(new(etapa, referencia, estado, fecha, alerta));
            if (!keys.ContainsKey(entidad)) keys[entidad] = new();
            keys[entidad].Add(registro);
        }
        var hoy = DateTime.Now;
        Add(nameof(SolicitudCotizacion), id, "Solicitud", $"SOL-{id:D5}", s.Estado, s.FechaSolicitud);
        foreach (var v in await _db.ValidacionesTecnicas.AsNoTracking().Where(x => x.SolicitudCotizacionId == id).ToListAsync())
            Add(nameof(ValidacionTecnica), v.Id, "Validación técnica", $"#{v.Id}", v.Resultado, v.Fecha);
        foreach (var v in await _db.VisitasTecnicas.AsNoTracking().Where(x => x.SolicitudCotizacionId == id).ToListAsync())
            Add(nameof(VisitaTecnica), v.Id, "Visita técnica", $"#{v.Id}", v.Estado, v.FechaHora, !v.VisitaCompletada && v.Estado != "Cancelada" && v.FechaHora < hoy ? "Visita atrasada" : null);
        var cot = await _db.Cotizaciones.AsNoTracking().Where(x => x.SolicitudCotizacionId == id).ToListAsync();
        foreach (var c in cot) Add(nameof(Cotizacion), c.Id, "Cotización", $"COT-{c.Id:D5}", c.Estado, c.FechaCreacion, c.Estado == "Pendiente" && c.FechaVencimiento < hoy ? "Cotización vencida" : null);
        foreach (var d in await _db.DisenosIA.AsNoTracking().Where(x => x.SolicitudCotizacionId == id).ToListAsync())
            Add(nameof(DisenoIA), d.Id, "Diseño IA", $"#{d.Id}", d.EstadoRevision, d.FechaGeneracion);
        var cids = cot.Select(x => x.Id).ToList();
        var compras = await _db.Compras.AsNoTracking().Where(x => cids.Contains(x.CotizacionId)).ToListAsync();
        foreach (var c in compras) Add(nameof(Compra), c.Id, "Compra", c.NumeroOrden, c.Estado, c.FechaCompra);
        var ids = compras.Select(x => x.Id).ToList();
        foreach (var f in await _db.Facturas.AsNoTracking().Where(x => ids.Contains(x.CompraId)).ToListAsync()) Add(nameof(Factura), f.Id, "Factura", f.NumeroFactura, f.Estado, f.FechaEmision);
        foreach (var p in await _db.OrdenesProduccion.AsNoTracking().Where(x => ids.Contains(x.CompraId)).ToListAsync()) Add(nameof(OrdenProduccion), p.Id, "Producción", p.NumeroOrden, p.Estado, p.FechaCreacion, p.Estado != "Terminada" && p.FechaFinPrevista.Date < hoy.Date ? "Producción atrasada" : null);
        foreach (var i in await _db.TrabajosInstalacion.AsNoTracking().Where(x => x.SolicitudCotizacionId == id || (x.CompraId.HasValue && ids.Contains(x.CompraId.Value))).ToListAsync())
        {
            Add(nameof(TrabajoInstalacion), i.Id, "Instalación", $"INS-{i.Id:D5}", i.Estado, i.FechaInstalacion, i.Estado != "Finalizada" && i.Estado != "Cancelada" && i.FechaInstalacion.Date < hoy.Date ? "Instalación atrasada" : null);
            if (i.FechaAprobacionCierre.HasValue) Add(nameof(TrabajoInstalacion), i.Id, "Cierre formal", $"INS-{i.Id:D5}", "Aprobado", i.FechaAprobacionCierre);
        }
        foreach (var k in keys)
        {
            var registros = k.Value.ToList(); var entidad = k.Key;
            m.Historial.AddRange(await _db.HistorialEstadosProyecto.AsNoTracking().Where(x => x.Entidad == entidad && registros.Contains(x.RegistroId)).ToListAsync());
        }
        m.Historial = m.Historial.OrderByDescending(x => x.FechaUtc).ToList();
        return View(m);
    }
}

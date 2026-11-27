using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Controllers
{
    [Authorize(Roles = "Administrador,Instalador")]
    public class CierreInstalacionesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _users;
        private readonly IWebHostEnvironment _environment;
        public CierreInstalacionesController(ApplicationDbContext db,
            UserManager<ApplicationUser> users, IWebHostEnvironment environment)
        {
            _db = db;
            _users = users;
            _environment = environment;
        }

        private async Task<TrabajoInstalacion?> BuscarAutorizado(int id)
        {
            var userId = _users.GetUserId(User);
            if (userId == null) return null;
            var query = _db.TrabajosInstalacion
                .Include(x => x.Evidencias).Include(x => x.HistorialCierre)
                .Include(x => x.Compra).Include(x => x.SolicitudCotizacion).AsQueryable();
            if (!User.IsInRole("Administrador"))
                query = query.Where(x => x.InstaladorId == userId);
            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }

        private static bool ChecklistCompleto(TrabajoInstalacion t) =>
            t.MedidasConfirmadas && t.MaterialListo && t.InstalacionIniciada &&
            t.InstalacionRealizada && t.RevisionAcabados && t.LimpiezaFinal;

        private void Registrar(TrabajoInstalacion t, string accion, string? comentario)
        {
            _db.HistorialCierresInstalacion.Add(new HistorialCierreInstalacion
            {
                TrabajoInstalacionId = t.Id,
                Accion = accion,
                Comentario = comentario,
                UsuarioId = _users.GetUserId(User) ?? "",
                Fecha = DateTime.UtcNow
            });
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var trabajo = await BuscarAutorizado(id);
            if (trabajo == null) return NotFound();
            ViewBag.EsAdministrador = User.IsInRole("Administrador");
            ViewBag.ChecklistCompleto = ChecklistCompleto(trabajo);
            return View(trabajo);
        }

        [HttpPost, ValidateAntiForgeryToken, RequestSizeLimit(6_000_000)]
        public async Task<IActionResult> SubirEvidencia(int id, IFormFile? archivo, string? descripcion)
        {
            var trabajo = await BuscarAutorizado(id);
            if (trabajo == null) return NotFound();
            if (trabajo.FechaAprobacionCierre != null || trabajo.FechaSolicitudCierre != null)
                return BadRequest("No se pueden adjuntar evidencias mientras el cierre esté solicitado o aprobado.");
            if (archivo == null || archivo.Length == 0 || archivo.Length > 5_000_000)
            {
                TempData["Error"] = "Seleccioná una fotografía de hasta 5 MB.";
                return RedirectToAction(nameof(Detalle), new { id });
            }
            if (descripcion?.Length > 300)
            {
                TempData["Error"] = "La descripción no puede superar 300 caracteres.";
                return RedirectToAction(nameof(Detalle), new { id });
            }
            // Comprobar la firma binaria, no confiar en el nombre ni en el ContentType enviado.
            var header = new byte[12];
            await using (var stream = archivo.OpenReadStream())
            {
                var read = await stream.ReadAsync(header.AsMemory());
                if (read < 12) return BadRequest("Archivo inválido.");
            }
            var jpg = header[0] == 0xff && header[1] == 0xd8 && header[2] == 0xff;
            var png = header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4e && header[3] == 0x47 && header[4] == 0x0d && header[5] == 0x0a && header[6] == 0x1a && header[7] == 0x0a;
            var webp = System.Text.Encoding.ASCII.GetString(header, 0, 4) == "RIFF" &&
                       System.Text.Encoding.ASCII.GetString(header, 8, 4) == "WEBP";
            if (!jpg && !png && !webp)
            {
                TempData["Error"] = "Solo se admiten fotografías JPG, PNG o WebP.";
                return RedirectToAction(nameof(Detalle), new { id });
            }
            var ext = jpg ? ".jpg" : png ? ".png" : ".webp";
            var mime = jpg ? "image/jpeg" : png ? "image/png" : "image/webp";
            var nombre = $"{Guid.NewGuid():N}{ext}";
            // Almacenamiento privado: no se exponen las fotos en wwwroot.
            var carpeta = Path.Combine(_environment.ContentRootPath, "App_Data", "EvidenciasInstalacion", id.ToString());
            Directory.CreateDirectory(carpeta);
            var ruta = Path.Combine(carpeta, nombre);
            await using (var destino = new FileStream(ruta, FileMode.CreateNew))
                await archivo.CopyToAsync(destino);
            try
            {
                _db.EvidenciasInstalacion.Add(new EvidenciaInstalacion
                {
                    TrabajoInstalacionId = id,
                    NombreArchivo = nombre,
                    TipoContenido = mime,
                    Descripcion = descripcion?.Trim(),
                    UsuarioId = _users.GetUserId(User) ?? "",
                    FechaRegistro = DateTime.UtcNow
                });
                Registrar(trabajo, "Evidencia agregada", descripcion?.Trim());
                await _db.SaveChangesAsync();
            }
            catch
            {
                System.IO.File.Delete(ruta);
                throw;
            }
            TempData["Mensaje"] = "Fotografía registrada correctamente.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Foto(int id, int evidenciaId)
        {
            var trabajo = await BuscarAutorizado(id);
            if (trabajo == null) return NotFound();
            var evidencia = trabajo.Evidencias.FirstOrDefault(x => x.Id == evidenciaId);
            if (evidencia == null) return NotFound();
            var ruta = Path.Combine(_environment.ContentRootPath, "App_Data", "EvidenciasInstalacion",
                id.ToString(), evidencia.NombreArchivo);
            if (!System.IO.File.Exists(ruta)) return NotFound();
            return PhysicalFile(ruta, evidencia.TipoContenido);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Solicitar(int id)
        {
            var trabajo = await BuscarAutorizado(id);
            if (trabajo == null) return NotFound();
            if (trabajo.FechaAprobacionCierre != null) return BadRequest("Cierre ya aprobado.");
            if (trabajo.FechaSolicitudCierre != null) return BadRequest("Ya se solicitó el cierre.");
            if (!ChecklistCompleto(trabajo) || !trabajo.Evidencias.Any())
            {
                TempData["Error"] = "Completá los seis pasos del checklist y adjuntá al menos una fotografía.";
                return RedirectToAction(nameof(Detalle), new { id });
            }
            trabajo.FechaSolicitudCierre = DateTime.UtcNow;
            trabajo.Estado = "Pendiente de cierre";
            Registrar(trabajo, "Cierre solicitado", "Checklist y evidencias presentadas");
            await _db.SaveChangesAsync();
            TempData["Mensaje"] = "Cierre enviado a revisión administrativa.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Aprobar(int id, string? comentario)
        {
            var trabajo = await BuscarAutorizado(id);
            if (trabajo == null) return NotFound();
            if (trabajo.FechaSolicitudCierre == null || trabajo.FechaAprobacionCierre != null ||
                !ChecklistCompleto(trabajo) || !trabajo.Evidencias.Any())
                return BadRequest("El cierre no cumple los requisitos para aprobarse.");
            if (comentario?.Length > 450) return BadRequest("Comentario demasiado largo.");
            trabajo.FechaAprobacionCierre = DateTime.UtcNow;
            trabajo.FechaFinalizacion = DateTime.Now;
            trabajo.AprobadoPorId = _users.GetUserId(User);
            trabajo.ObservacionCierre = comentario?.Trim();
            trabajo.Estado = "Finalizada";
            if (trabajo.Compra != null) trabajo.Compra.Estado = "Finalizada";
            if (trabajo.SolicitudCotizacion != null) trabajo.SolicitudCotizacion.Estado = "Finalizado";
            Registrar(trabajo, "Cierre aprobado", comentario?.Trim());
            await _db.SaveChangesAsync();
            TempData["Mensaje"] = "Instalación cerrada y aprobada.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Rechazar(int id, string? comentario)
        {
            var trabajo = await BuscarAutorizado(id);
            if (trabajo == null) return NotFound();
            if (trabajo.FechaSolicitudCierre == null || trabajo.FechaAprobacionCierre != null)
                return BadRequest("No existe una solicitud de cierre pendiente.");
            if (string.IsNullOrWhiteSpace(comentario) || comentario.Length > 450)
            {
                TempData["Error"] = "Indicá el motivo del rechazo (máximo 450 caracteres).";
                return RedirectToAction(nameof(Detalle), new { id });
            }
            trabajo.FechaSolicitudCierre = null;
            trabajo.Estado = "En proceso";
            trabajo.ObservacionCierre = comentario.Trim();
            Registrar(trabajo, "Correcciones solicitadas", comentario.Trim());
            await _db.SaveChangesAsync();
            TempData["Mensaje"] = "Se solicitó corregir la instalación.";
            return RedirectToAction(nameof(Detalle), new { id });
        }
    }
}

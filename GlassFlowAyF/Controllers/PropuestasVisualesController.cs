using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using GlassFlowAyF.Models.ViewModels;
using GlassFlowAyF.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GlassFlowAyF.Controllers;

/// <summary>
/// Módulo 14 - Diseño y Propuesta Visual.
/// Administrador = cotizador (crea, edita, adjunta). Cliente = solo consulta lo suyo.
/// </summary>
[Authorize(Roles = "Administrador,Cliente")]
public class PropuestasVisualesController(
    ApplicationDbContext context,
    UserManager<ApplicationUser> users,
    IPropuestaVisualAlmacenamiento almacenamiento,
    IWebHostEnvironment env) : Controller
{
    private const string MensajeInsuficiente =
        "Información insuficiente para generar la propuesta visual. Por favor complete los datos técnicos requeridos";

    private const string MensajeFormato =
        "Formato de archivo no válido. Únicamente se admiten archivos en formato .JPG, .PNG, .PDF o .DWG menores a 15 MB";

    private const long LimiteSolicitud = 150_000_000;

    // =====================================================
    // ADMINISTRADOR - LISTADO GENERAL
    // =====================================================

    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Index(int? solicitudId)
    {
        var consulta = context.PropuestasVisuales
            .AsNoTracking()
            .Include(p => p.SolicitudCotizacion)
            .Include(p => p.Archivos)
            .AsQueryable();

        if (solicitudId.HasValue)
            consulta = consulta.Where(p => p.SolicitudCotizacionId == solicitudId.Value);

        var propuestas = await consulta
            .OrderByDescending(p => p.FechaActualizacion ?? p.FechaCreacion)
            .ToListAsync();

        // Solicitudes que todavía no tienen propuesta (para crear desde aquí).
        ViewBag.SinPropuesta = await context.SolicitudesCotizacion
            .AsNoTracking()
            .Include(s => s.Producto)
            .Where(s => s.Estado != "Borrador" && !s.PropuestasVisuales.Any())
            .OrderByDescending(s => s.FechaSolicitud)
            .Take(15)
            .ToListAsync();

        ViewBag.SolicitudId = solicitudId;
        return View(propuestas);
    }

    // =====================================================
    // CLIENTE - MIS PROYECTOS
    // =====================================================

    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> MisPropuestas()
    {
        var usuario = await users.GetUserAsync(User);
        if (usuario == null) return RedirectToAction("Login", "Account");

        var solicitudes = await context.SolicitudesCotizacion
            .AsNoTracking()
            .Include(s => s.Producto)
            .Include(s => s.PropuestasVisuales)
            .Where(s => s.Correo == usuario.Email && s.Estado != "Borrador")
            .OrderByDescending(s => s.FechaSolicitud)
            .ToListAsync();

        return View(solicitudes);
    }

    // =====================================================
    // PROPUESTAS DE UNA SOLICITUD (cliente y administrador)
    // =====================================================

    public async Task<IActionResult> PorSolicitud(int solicitudId)
    {
        var solicitud = await CargarSolicitudAsync(solicitudId);
        if (solicitud == null) return NotFound();
        if (!await TienePermisoAsync(solicitud)) return Forbid();

        var propuestas = await context.PropuestasVisuales
            .AsNoTracking()
            .Include(p => p.Archivos)
            .Where(p => p.SolicitudCotizacionId == solicitudId)
            .OrderByDescending(p => p.FechaActualizacion ?? p.FechaCreacion)
            .ToListAsync();

        ViewBag.Solicitud = solicitud;
        return View(propuestas);
    }

    // =====================================================
    // CREAR
    // =====================================================

    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public async Task<IActionResult> Crear(int solicitudId)
    {
        var solicitud = await CargarSolicitudAsync(solicitudId);
        if (solicitud == null) return NotFound();

        PrepararFormulario(solicitud);

        // Criterio 2: si faltan insumos se alerta desde que intenta crear.
        if ((ViewBag.Faltantes as List<string>)?.Count > 0)
            ModelState.AddModelError(string.Empty, MensajeInsuficiente);

        return View(new PropuestaVisualFormViewModel { SolicitudId = solicitudId });
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost, ValidateAntiForgeryToken]
    [RequestSizeLimit(LimiteSolicitud)]
    [RequestFormLimits(MultipartBodyLengthLimit = LimiteSolicitud)]
    public async Task<IActionResult> Crear(PropuestaVisualFormViewModel model, CancellationToken ct)
    {
        var solicitud = await CargarSolicitudAsync(model.SolicitudId);
        if (solicitud == null) return NotFound();

        PrepararFormulario(solicitud);

        if ((ViewBag.Faltantes as List<string>)?.Count > 0)
        {
            ModelState.AddModelError(string.Empty, MensajeInsuficiente);
            return View(model);
        }

        var diseno = ValidarDiseno(solicitud, model);
        var archivos = model.Archivos?.Where(a => a != null && a.Length > 0).ToList() ?? new List<IFormFile>();
        await ValidarArchivosAsync(archivos);

        if (!ModelState.IsValid) return View(model);

        var propuesta = new PropuestaVisual
        {
            SolicitudCotizacionId = solicitud.Id,
            DisenoIAId = diseno?.Id,
            Titulo = model.Titulo.Trim(),
            Descripcion = model.Descripcion.Trim(),
            RutaImagenPrincipal = diseno?.RutaImagenGenerada,
            CreadaPor = UsuarioActual()
        };

        var guardados = new List<string>();
        await using var transaccion = await context.Database.BeginTransactionAsync(ct);

        try
        {
            // 1) Se guarda para obtener el Id y construir el código PROP-VIS-001.
            context.PropuestasVisuales.Add(propuesta);
            await context.SaveChangesAsync(ct);

            propuesta.Codigo = $"PROP-VIS-{propuesta.Id:000}";
            Bitacora(propuesta.Id, "Creación", $"Propuesta {propuesta.Codigo} {propuesta.VersionTexto} registrada.");

            // 2) Adjuntos iniciales (opcionales).
            foreach (var archivo in archivos)
            {
                var resultado = await almacenamiento.GuardarAsync(propuesta.Id, archivo, ct);
                guardados.Add(resultado.RutaRelativa);
                context.PropuestaVisualArchivos.Add(NuevoArchivo(propuesta.Id, resultado));
            }

            if (archivos.Count > 0)
                Bitacora(propuesta.Id, "Archivos adjuntados", $"{archivos.Count} archivo(s) adjuntado(s) al crear la propuesta.");

            await context.SaveChangesAsync(ct);
            await transaccion.CommitAsync(ct);
        }
        catch
        {
            await transaccion.RollbackAsync(ct);
            foreach (var ruta in guardados) almacenamiento.Eliminar(ruta);
            throw;
        }

        TempData["Mensaje"] = $"Propuesta visual {propuesta.Codigo} creada correctamente. Ya puede revisarse la vista previa.";
        return RedirectToAction(nameof(Detalle), new { id = propuesta.Id });
    }

    // =====================================================
    // EDITAR (incrementa versión y registra bitácora)
    // =====================================================

    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var propuesta = await context.PropuestasVisuales.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (propuesta == null) return NotFound();

        var solicitud = await CargarSolicitudAsync(propuesta.SolicitudCotizacionId);
        if (solicitud == null) return NotFound();

        PrepararFormulario(solicitud);
        ViewBag.Propuesta = propuesta;

        return View(new PropuestaVisualFormViewModel
        {
            Id = propuesta.Id,
            SolicitudId = propuesta.SolicitudCotizacionId,
            Titulo = propuesta.Titulo,
            Descripcion = propuesta.Descripcion,
            DisenoIAId = propuesta.DisenoIAId,
            RowVersion = propuesta.RowVersion
        });
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost, ValidateAntiForgeryToken]
    [RequestSizeLimit(LimiteSolicitud)]
    [RequestFormLimits(MultipartBodyLengthLimit = LimiteSolicitud)]
    public async Task<IActionResult> Editar(PropuestaVisualFormViewModel model, CancellationToken ct)
    {
        var propuesta = await context.PropuestasVisuales.FirstOrDefaultAsync(p => p.Id == model.Id, ct);
        if (propuesta == null) return NotFound();

        var solicitud = await CargarSolicitudAsync(propuesta.SolicitudCotizacionId);
        if (solicitud == null) return NotFound();

        PrepararFormulario(solicitud);
        ViewBag.Propuesta = propuesta;

        var diseno = ValidarDiseno(solicitud, model);
        var archivos = model.Archivos?.Where(a => a != null && a.Length > 0).ToList() ?? new List<IFormFile>();
        await ValidarArchivosAsync(archivos);

        if (!ModelState.IsValid) return View(model);

        // Detecta qué cambió realmente.
        var cambios = new List<string>();
        var titulo = model.Titulo.Trim();
        var descripcion = model.Descripcion.Trim();
        if (propuesta.Titulo != titulo) cambios.Add("título");
        if (propuesta.Descripcion != descripcion) cambios.Add("descripción");
        if (propuesta.DisenoIAId != diseno?.Id) cambios.Add("diseño IA base");
        if (archivos.Count > 0) cambios.Add($"{archivos.Count} archivo(s) nuevo(s)");

        if (cambios.Count == 0)
        {
            TempData["Error"] = "No hay cambios para guardar.";
            return RedirectToAction(nameof(Detalle), new { id = propuesta.Id });
        }

        var versionAnterior = propuesta.VersionTexto;

        // Si otra persona editó antes, EF lanza DbUpdateConcurrencyException.
        context.Entry(propuesta).Property(p => p.RowVersion).OriginalValue = model.RowVersion;

        propuesta.Titulo = titulo;
        propuesta.Descripcion = descripcion;
        propuesta.DisenoIAId = diseno?.Id;
        propuesta.RutaImagenPrincipal = diseno?.RutaImagenGenerada;
        propuesta.VersionMenor += 1;
        propuesta.FechaActualizacion = DateTime.Now;
        propuesta.RowVersion = Guid.NewGuid();

        Bitacora(propuesta.Id, "Actualización",
            $"Versión {versionAnterior} → {propuesta.VersionTexto}. Cambios: {string.Join(", ", cambios)}.");

        var guardados = new List<string>();

        try
        {
            foreach (var archivo in archivos)
            {
                var resultado = await almacenamiento.GuardarAsync(propuesta.Id, archivo, ct);
                guardados.Add(resultado.RutaRelativa);
                context.PropuestaVisualArchivos.Add(NuevoArchivo(propuesta.Id, resultado));
            }

            await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            foreach (var ruta in guardados) almacenamiento.Eliminar(ruta);
            ModelState.AddModelError(string.Empty,
                "Otra persona modificó esta propuesta mientras la editaba. Recargue la página e intente de nuevo.");
            return View(model);
        }
        catch
        {
            foreach (var ruta in guardados) almacenamiento.Eliminar(ruta);
            throw;
        }

        TempData["Mensaje"] = $"Propuesta actualizada. Nueva versión: {propuesta.VersionTexto}.";
        return RedirectToAction(nameof(Detalle), new { id = propuesta.Id });
    }

    // =====================================================
    // DETALLE / VISOR (cliente y administrador)
    // =====================================================

    public async Task<IActionResult> Detalle(int id)
    {
        var propuesta = await CargarPropuestaCompletaAsync(id);
        if (propuesta == null) return NotFound();
        if (!await TienePermisoAsync(propuesta.SolicitudCotizacion)) return Forbid();

        return View(propuesta);
    }

    // =====================================================
    // ADJUNTAR ARCHIVOS
    // =====================================================

    [Authorize(Roles = "Administrador")]
    [HttpPost, ValidateAntiForgeryToken]
    [RequestSizeLimit(LimiteSolicitud)]
    [RequestFormLimits(MultipartBodyLengthLimit = LimiteSolicitud)]
    public async Task<IActionResult> Adjuntar(int propuestaId, List<IFormFile>? archivos, CancellationToken ct)
    {
        var propuesta = await context.PropuestasVisuales.FirstOrDefaultAsync(p => p.Id == propuestaId, ct);
        if (propuesta == null) return NotFound();

        var seleccionados = archivos?.Where(a => a != null && a.Length > 0).ToList() ?? new List<IFormFile>();

        if (seleccionados.Count == 0)
        {
            TempData["Error"] = "Seleccione al menos un archivo para adjuntar.";
            return RedirectToAction(nameof(Detalle), new { id = propuestaId });
        }

        var errores = await almacenamiento.ValidarAsync(seleccionados);

        if (errores.Count > 0)
        {
            // Criterio 2: se bloquea la carga y se resalta qué archivo falló y por qué.
            TempData["Error"] = MensajeFormato + ". Rechazados: " +
                string.Join("; ", errores.Select(e => $"{e.Nombre} ({e.Motivo})"));
            return RedirectToAction(nameof(Detalle), new { id = propuestaId });
        }

        var guardados = new List<string>();

        try
        {
            foreach (var archivo in seleccionados)
            {
                var resultado = await almacenamiento.GuardarAsync(propuestaId, archivo, ct);
                guardados.Add(resultado.RutaRelativa);
                context.PropuestaVisualArchivos.Add(NuevoArchivo(propuestaId, resultado));
            }

            Bitacora(propuestaId, "Archivos adjuntados",
                string.Join(", ", seleccionados.Select(a => Path.GetFileName(a.FileName))));

            await context.SaveChangesAsync(ct);
        }
        catch
        {
            foreach (var ruta in guardados) almacenamiento.Eliminar(ruta);
            throw;
        }

        TempData["Mensaje"] = $"Se cargaron {seleccionados.Count} archivo(s) correctamente: " +
            string.Join(", ", seleccionados.Select(a => Path.GetFileName(a.FileName)));
        return RedirectToAction(nameof(Detalle), new { id = propuestaId });
    }

    // =====================================================
    // ELIMINAR ADJUNTO
    // =====================================================

    [Authorize(Roles = "Administrador")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarArchivo(int archivoId, CancellationToken ct)
    {
        var archivo = await context.PropuestaVisualArchivos.FirstOrDefaultAsync(a => a.Id == archivoId, ct);
        if (archivo == null) return NotFound();

        var propuestaId = archivo.PropuestaVisualId;
        var nombre = archivo.NombreOriginal ?? archivo.RutaArchivo;

        almacenamiento.Eliminar(archivo.RutaArchivo);
        context.PropuestaVisualArchivos.Remove(archivo);
        Bitacora(propuestaId, "Archivo eliminado", nombre);
        await context.SaveChangesAsync(ct);

        TempData["Mensaje"] = "El archivo se eliminó y se desvinculó de la propuesta.";
        return RedirectToAction(nameof(Detalle), new { id = propuestaId });
    }

    // =====================================================
    // ENLACE SEGURO A UN ADJUNTO (solo administrador o dueño)
    // =====================================================

    public async Task<IActionResult> Archivo(int id, bool descargar = false)
    {
        var archivo = await context.PropuestaVisualArchivos
            .AsNoTracking()
            .Include(a => a.PropuestaVisual)
                .ThenInclude(p => p!.SolicitudCotizacion)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (archivo?.PropuestaVisual?.SolicitudCotizacion == null) return NotFound();
        if (!await TienePermisoAsync(archivo.PropuestaVisual.SolicitudCotizacion)) return Forbid();

        var ruta = almacenamiento.ObtenerRutaFisica(archivo.RutaArchivo);
        if (!System.IO.File.Exists(ruta)) return NotFound();

        var tipo = archivo.Extension switch
        {
            ".pdf" => "application/pdf",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            _ => "application/octet-stream"
        };

        Response.Headers["X-Content-Type-Options"] = "nosniff";

        var nombreDescarga = archivo.NombreOriginal ?? $"archivo{archivo.Extension}";

        return descargar || tipo == "application/octet-stream"
            ? PhysicalFile(ruta, tipo, nombreDescarga)
            : PhysicalFile(ruta, tipo);
    }

    // =====================================================
    // DESCARGA EN PDF CON MEMBRETE
    // =====================================================

    public async Task<IActionResult> DescargarPdf(int id)
    {
        var propuesta = await CargarPropuestaCompletaAsync(id);
        if (propuesta == null) return NotFound();
        if (!await TienePermisoAsync(propuesta.SolicitudCotizacion)) return Forbid();

        byte[] pdf;

        try
        {
            pdf = GenerarPdf(propuesta);
        }
        catch (Exception)
        {
            TempData["Error"] = "No se pudo generar el PDF. Verifique que las imágenes adjuntas no estén dañadas.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        return File(pdf, "application/pdf", $"{propuesta.Codigo}_{propuesta.VersionTexto}.pdf");
    }

    // =====================================================
    // AUXILIARES
    // =====================================================

    private string UsuarioActual() => User.Identity?.Name ?? "Administrador";

    private async Task<bool> TienePermisoAsync(SolicitudCotizacion? solicitud)
    {
        if (solicitud == null) return false;
        if (User.IsInRole("Administrador")) return true;

        var usuario = await users.GetUserAsync(User);
        return usuario != null &&
               string.Equals(usuario.Email, solicitud.Correo, StringComparison.OrdinalIgnoreCase);
    }

    private Task<SolicitudCotizacion?> CargarSolicitudAsync(int id) =>
        context.SolicitudesCotizacion
            .AsNoTracking()
            .Include(s => s.Producto)
            .Include(s => s.Material)
            .Include(s => s.Fotografias)
            .Include(s => s.ValidacionesTecnicas)
            .Include(s => s.DisenosIA)
            .FirstOrDefaultAsync(s => s.Id == id);

    private Task<PropuestaVisual?> CargarPropuestaCompletaAsync(int id) =>
        context.PropuestasVisuales
            .AsNoTracking()
            .Include(p => p.SolicitudCotizacion).ThenInclude(s => s!.Producto)
            .Include(p => p.SolicitudCotizacion).ThenInclude(s => s!.Material)
            .Include(p => p.DisenoIA)
            .Include(p => p.Archivos)
            .Include(p => p.Bitacora)
            .FirstOrDefaultAsync(p => p.Id == id);

    /// <summary>Insumos mínimos para poder crear la propuesta (criterio 2 de la HU 1).</summary>
    private static List<string> ObtenerFaltantes(SolicitudCotizacion solicitud)
    {
        var faltantes = new List<string>();

        var ultimaValidacion = solicitud.ValidacionesTecnicas
            .OrderByDescending(v => v.Fecha)
            .ThenByDescending(v => v.Id)
            .FirstOrDefault();

        if (ultimaValidacion?.Resultado != "Validada")
            faltantes.Add("Validación técnica de la solicitud");

        if (!(solicitud.Ancho > 0) || !(solicitud.Alto > 0))
            faltantes.Add("Medidas finales (ancho y alto)");

        if (!solicitud.Fotografias.Any())
            faltantes.Add("Fotografías del espacio o plano base");

        return faltantes;
    }

    private void PrepararFormulario(SolicitudCotizacion solicitud)
    {
        ViewBag.Solicitud = solicitud;
        ViewBag.Faltantes = ObtenerFaltantes(solicitud);
        ViewBag.Disenos = solicitud.DisenosIA.OrderByDescending(d => d.FechaGeneracion).ToList();
    }

    private DisenoIA? ValidarDiseno(SolicitudCotizacion solicitud, PropuestaVisualFormViewModel model)
    {
        if (!model.DisenoIAId.HasValue) return null;

        var diseno = solicitud.DisenosIA.FirstOrDefault(d => d.Id == model.DisenoIAId.Value);

        if (diseno == null)
            ModelState.AddModelError(nameof(model.DisenoIAId), "El diseño seleccionado no pertenece a esta solicitud.");

        return diseno;
    }

    private async Task ValidarArchivosAsync(List<IFormFile> archivos)
    {
        if (archivos.Count == 0) return;

        var errores = await almacenamiento.ValidarAsync(archivos);
        if (errores.Count == 0) return;

        ModelState.AddModelError(nameof(PropuestaVisualFormViewModel.Archivos),
            MensajeFormato + ". Rechazados: " +
            string.Join("; ", errores.Select(e => $"{e.Nombre} ({e.Motivo})")));
    }

    private void Bitacora(int propuestaId, string accion, string? detalle)
    {
        context.PropuestaVisualBitacoras.Add(new PropuestaVisualBitacora
        {
            PropuestaVisualId = propuestaId,
            Accion = accion,
            Detalle = detalle != null && detalle.Length > 500 ? detalle[..500] : detalle,
            Usuario = UsuarioActual(),
            Fecha = DateTime.Now
        });
    }

    private PropuestaVisualArchivo NuevoArchivo(int propuestaId, ArchivoGuardado r) => new()
    {
        PropuestaVisualId = propuestaId,
        RutaArchivo = r.RutaRelativa,
        NombreOriginal = r.NombreOriginal.Length > 255 ? r.NombreOriginal[..255] : r.NombreOriginal,
        TipoContenido = r.TipoContenido.Length > 100 ? r.TipoContenido[..100] : r.TipoContenido,
        Extension = r.Extension,
        TamanoBytes = r.Tamano,
        SubidoPor = UsuarioActual(),
        FechaCarga = DateTime.Now
    };

    // =====================================================
    // PDF (QuestPDF)
    // =====================================================

    private byte[] GenerarPdf(PropuestaVisual p)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var s = p.SolicitudCotizacion!;
        var imagenes = new List<(string Titulo, byte[] Datos)>();

        // Imagen del diseño IA (vive en wwwroot).
        if (!string.IsNullOrWhiteSpace(p.DisenoIA?.RutaImagenGenerada))
        {
            var rutaWeb = p.DisenoIA!.RutaImagenGenerada!.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var rutaFisica = Path.GetFullPath(Path.Combine(env.WebRootPath, rutaWeb));

            if (rutaFisica.StartsWith(Path.GetFullPath(env.WebRootPath), StringComparison.OrdinalIgnoreCase) &&
                System.IO.File.Exists(rutaFisica))
            {
                imagenes.Add(("Diseño generado con IA", System.IO.File.ReadAllBytes(rutaFisica)));
            }
        }

        // Imágenes adjuntas (jpg/png).
        foreach (var a in p.Archivos.Where(a => a.Extension is ".jpg" or ".jpeg" or ".png"))
        {
            var ruta = almacenamiento.ObtenerRutaFisica(a.RutaArchivo);
            if (System.IO.File.Exists(ruta))
                imagenes.Add((a.NombreOriginal ?? "Imagen adjunta", System.IO.File.ReadAllBytes(ruta)));
        }

        var otros = p.Archivos.Where(a => a.Extension is ".pdf" or ".dwg").ToList();

        var medidas = $"{s.Ancho:0.##} x {s.Alto:0.##}" +
                      (s.Profundidad.HasValue ? $" x {s.Profundidad:0.##}" : "") +
                      $" {s.UnidadMedida}";

        return Document.Create(documento =>
        {
            documento.Page(pagina =>
            {
                pagina.Size(PageSizes.A4);
                pagina.Margin(35);
                pagina.DefaultTextStyle(t => t.FontSize(10));

                pagina.Header().Column(col =>
                {
                    col.Item().Row(fila =>
                    {
                        fila.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Soluciones A&F").FontSize(20).Bold().FontColor(Colors.Blue.Darken3);
                            c.Item().Text("Diseño e instalación de productos en vidrio y aluminio")
                                .FontColor(Colors.Grey.Darken1);
                        });

                        fila.ConstantItem(150).AlignRight().Column(c =>
                        {
                            c.Item().Text(p.Codigo ?? "").Bold();
                            c.Item().Text(p.VersionTexto);
                            c.Item().Text(DateTime.Now.ToString("dd/MM/yyyy"));
                        });
                    });

                    col.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Blue.Darken3);
                });

                pagina.Content().PaddingVertical(10).Column(col =>
                {
                    col.Spacing(8);

                    col.Item().Text("Propuesta visual preliminar").FontSize(16).Bold();
                    col.Item().Text(p.Titulo).FontSize(13).SemiBold();
                    col.Item().Text(p.Descripcion);

                    col.Item().PaddingTop(6).Text("Datos del proyecto").FontSize(12).Bold();
                    col.Item().Text(t => { t.Span("Solicitud: ").SemiBold(); t.Span($"SOL-{s.Id:D5}"); });
                    col.Item().Text(t => { t.Span("Cliente: ").SemiBold(); t.Span(s.NombreCliente); });
                    col.Item().Text(t => { t.Span("Producto: ").SemiBold(); t.Span(s.Producto?.Nombre ?? s.TipoProducto); });
                    col.Item().Text(t => { t.Span("Material y acabado: ").SemiBold(); t.Span(s.Material?.Nombre ?? "Por definir"); });
                    col.Item().Text(t => { t.Span("Medidas: ").SemiBold(); t.Span(medidas); });
                    col.Item().Text(t => { t.Span("Área de instalación: ").SemiBold(); t.Span(s.UbicacionInstalacion); });

                    if (p.DisenoIA != null)
                    {
                        col.Item().Text(t =>
                        {
                            t.Span("Estilo / color de perfilería: ").SemiBold();
                            t.Span($"{p.DisenoIA.Estilo} / {p.DisenoIA.ColorPerfil ?? "N/D"}");
                        });
                    }

                    foreach (var (titulo, datos) in imagenes)
                    {
                        col.Item().PaddingTop(6).Text(titulo).SemiBold();
                        col.Item().MaxHeight(330).Image(datos).FitArea();
                    }

                    if (otros.Count > 0)
                    {
                        col.Item().PaddingTop(6).Text("Documentos de referencia adjuntos").SemiBold();
                        foreach (var o in otros)
                            col.Item().Text($"• {o.NombreOriginal} ({o.Extension.TrimStart('.').ToUpperInvariant()})");
                    }

                    col.Item().PaddingTop(10).Text(
                        "Esta propuesta es una representación visual preliminar. Las medidas, materiales y " +
                        "acabados definitivos se confirman en la cotización y en el proceso de aprobación.")
                        .Italic().FontSize(9).FontColor(Colors.Grey.Darken1);
                });

                pagina.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Soluciones A&F · Página ");
                    t.CurrentPageNumber();
                    t.Span(" de ");
                    t.TotalPages();
                });
            });
        }).GeneratePdf();
    }
}

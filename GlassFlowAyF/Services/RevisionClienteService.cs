using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using GlassFlowAyF.Data;
using GlassFlowAyF.Models;
using GlassFlowAyF.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GlassFlowAyF.Services;

/// <summary>
/// Módulo 15 – Revisión y Aprobación del Cliente.
/// Aplica la máquina de estados, la auditoría (IP, fecha, usuario, snapshot) y genera la orden preliminar.
/// </summary>
public class RevisionClienteService : IRevisionClienteService
{
    private const string MensajeConflicto =
        "La propuesta ya fue respondida o se modificó desde otra sesión. Recargue la página para ver la versión vigente.";

    private readonly ApplicationDbContext _context;
    private readonly INotificacionService _notificaciones;
    private readonly ILogger<RevisionClienteService> _logger;

    public RevisionClienteService(ApplicationDbContext context, INotificacionService notificaciones,
        ILogger<RevisionClienteService> logger)
    {
        _context = context;
        _notificaciones = notificaciones;
        _logger = logger;
    }

    // =====================================================================
    // CONSULTA (HU 1)
    // =====================================================================

    public async Task<ResultadoConsulta> ObtenerRevisionAsync(int solicitudId, string usuarioId,
        bool esAdministrador, int? cotizacionId = null)
    {
        var solicitud = await _context.SolicitudesCotizacion
            .AsNoTracking()
            .Include(s => s.Producto)
            .Include(s => s.Material)
            .Include(s => s.Fotografias)
            .Include(s => s.DisenosIA)
            .Include(s => s.Cotizaciones).ThenInclude(c => c.Compra)
            .Include(s => s.Cotizaciones).ThenInclude(c => c.OrdenPreliminar)
            .AsSplitQuery()
            .FirstOrDefaultAsync(s => s.Id == solicitudId);

        if (solicitud == null) return new ResultadoConsulta(CodigoRevision.NoEncontrado);

        if (!esAdministrador && !await EsPropietarioAsync(solicitud, usuarioId))
            return new ResultadoConsulta(CodigoRevision.Prohibido);

        var cotizacion = cotizacionId.HasValue
            ? solicitud.Cotizaciones.FirstOrDefault(c => c.Id == cotizacionId.Value)
            : solicitud.Cotizaciones
                .OrderByDescending(c => c.Activa)
                .ThenByDescending(c => c.FechaCreacion)
                .ThenByDescending(c => c.Id)
                .FirstOrDefault();

        if (cotizacionId.HasValue && cotizacion == null)
            return new ResultadoConsulta(CodigoRevision.NoEncontrado);

        var historial = await _context.RevisionesCliente
            .AsNoTracking()
            .Include(r => r.Usuario)
            .Where(r => r.SolicitudCotizacionId == solicitudId)
            .OrderByDescending(r => r.FechaUtc)
            .ToListAsync();

        var modelo = new RevisionPropuestaViewModel
        {
            Solicitud = solicitud,
            Cotizacion = cotizacion,
            Diseno = cotizacion == null ? null : ResolverDiseno(solicitud, cotizacion),
            Orden = cotizacion?.OrdenPreliminar,
            Historial = historial,
            EsAdministrador = esAdministrador,
            EsCliente = !esAdministrador
        };

        return new ResultadoConsulta(CodigoRevision.Ok, modelo);
    }

    // =====================================================================
    // DECISIONES (HU 2 y HU 3)
    // =====================================================================

    public Task<ResultadoRevision> AprobarAsync(ComandoRevision comando) =>
        EjecutarAsync(comando, EstadosRevision.DecisionAprobada);

    public Task<ResultadoRevision> SolicitarModificacionAsync(ComandoRevision comando) =>
        EjecutarAsync(comando, EstadosRevision.DecisionModificacion);

    public Task<ResultadoRevision> RechazarAsync(ComandoRevision comando) =>
        EjecutarAsync(comando, EstadosRevision.DecisionRechazada);

    private async Task<ResultadoRevision> EjecutarAsync(ComandoRevision cmd, string decision)
    {
        // ---- Validaciones de entrada (antes de tocar la BD) ----
        var comentario = cmd.Comentario?.Trim();

        if (decision == EstadosRevision.DecisionAprobada)
        {
            if (!cmd.AceptoTerminos)
                return new ResultadoRevision(CodigoRevision.Invalido,
                    "Debe aceptar los términos y condiciones para aprobar la propuesta.");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(comentario) || comentario.Length < EstadosRevision.ComentarioMinimo)
                return new ResultadoRevision(CodigoRevision.Invalido, EstadosRevision.MensajeComentarioObligatorio);
        }

        if (comentario?.Length > EstadosRevision.ComentarioMaximo)
            return new ResultadoRevision(CodigoRevision.Invalido,
                $"El comentario no puede superar {EstadosRevision.ComentarioMaximo} caracteres.");

        await using var tx = await _context.Database.BeginTransactionAsync();

        var cotizacion = await _context.Cotizaciones
            .Include(c => c.SolicitudCotizacion).ThenInclude(s => s!.Material)
            .Include(c => c.SolicitudCotizacion).ThenInclude(s => s!.Producto)
            .Include(c => c.SolicitudCotizacion).ThenInclude(s => s!.DisenosIA)
            .Include(c => c.Compra)
            .Include(c => c.OrdenPreliminar)
            .FirstOrDefaultAsync(c => c.Id == cmd.CotizacionId);

        if (cotizacion?.SolicitudCotizacion == null)
            return new ResultadoRevision(CodigoRevision.NoEncontrado, "La propuesta no existe.");

        var solicitud = cotizacion.SolicitudCotizacion;

        // ---- Autorización: solo el cliente dueño de la solicitud ----
        if (!await EsPropietarioAsync(solicitud, cmd.UsuarioId))
            return new ResultadoRevision(CodigoRevision.Prohibido, "No tiene permiso para responder esta propuesta.", solicitud.Id);

        // ---- Máquina de estados: única transición válida desde Pendiente ----
        if (cotizacion.Version != cmd.Version)
            return new ResultadoRevision(CodigoRevision.Conflicto, MensajeConflicto, solicitud.Id);

        if (cotizacion.Congelada || !cotizacion.Activa || cotizacion.Estado != EstadosRevision.CotPendiente
            || cotizacion.Compra != null || cotizacion.OrdenPreliminar != null)
        {
            var mensaje = cotizacion.Estado == EstadosRevision.CotCambios
                ? "La aprobación está deshabilitada mientras el cotizador atiende su solicitud de modificación."
                : "Esta propuesta ya fue respondida y no admite nuevas decisiones.";
            return new ResultadoRevision(CodigoRevision.Conflicto, mensaje, solicitud.Id);
        }

        if (cotizacion.FechaVencimiento.Date < DateTime.Today)
            return new ResultadoRevision(CodigoRevision.Invalido, "Esta cotización ha vencido.", solicitud.Id);

        // ---- Transición ----
        var diseno = ResolverDiseno(solicitud, cotizacion);
        cotizacion.DisenoIAId ??= diseno?.Id;

        var estadoCotAnterior = cotizacion.Estado;
        var estadoSolAnterior = solicitud.Estado;
        var versionRespondida = cotizacion.Version;
        var ahora = DateTime.Now;

        cotizacion.ComentarioCliente = comentario;
        cotizacion.FechaRespuestaCliente = ahora;

        OrdenPreliminar? orden = null;
        switch (decision)
        {
            case EstadosRevision.DecisionAprobada:
                cotizacion.Estado = EstadosRevision.CotAprobada;
                cotizacion.Congelada = true;
                cotizacion.FechaCongelacion = ahora;
                solicitud.Estado = EstadosRevision.AprobadoCliente;
                orden = new OrdenPreliminar
                {
                    CotizacionId = cotizacion.Id,
                    SolicitudCotizacionId = solicitud.Id,
                    Numero = $"GF-OP-{ahora:yyyy}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
                    Estado = "Preliminar",
                    FechaCreacion = ahora
                };
                _context.OrdenesPreliminares.Add(orden);
                break;

            case EstadosRevision.DecisionModificacion:
                cotizacion.Estado = EstadosRevision.CotCambios;
                solicitud.Estado = EstadosRevision.SolicitudModificacion;
                break;

            case EstadosRevision.DecisionRechazada:
                cotizacion.Estado = EstadosRevision.CotRechazada;
                cotizacion.Activa = false;
                solicitud.Estado = EstadosRevision.RechazadaCliente;
                break;
        }

        var snapshot = CrearSnapshot(cotizacion, solicitud, diseno);
        if (decision == EstadosRevision.DecisionAprobada)
            cotizacion.SnapshotHash = Sha256(snapshot);

        // Nueva versión: cualquier formulario abierto con la versión anterior queda invalidado.
        cotizacion.Version = Guid.NewGuid();

        _context.RevisionesCliente.Add(new RevisionCliente
        {
            CotizacionId = cotizacion.Id,
            SolicitudCotizacionId = solicitud.Id,
            DisenoIAId = diseno?.Id,
            UsuarioId = cmd.UsuarioId,
            Decision = decision,
            Comentario = comentario,
            EstadoCotizacionAnterior = estadoCotAnterior,
            EstadoCotizacionNuevo = cotizacion.Estado,
            EstadoSolicitudAnterior = estadoSolAnterior,
            EstadoSolicitudNuevo = solicitud.Estado,
            VersionCotizacion = versionRespondida,
            AceptoTerminos = cmd.AceptoTerminos && decision == EstadosRevision.DecisionAprobada,
            DireccionIp = Recortar(cmd.DireccionIp, 45),
            UserAgent = Recortar(cmd.UserAgent, 300),
            FechaUtc = DateTime.UtcNow,
            SnapshotJson = snapshot
        });

        _notificaciones.AgregarInternas(decision, solicitud, cotizacion, comentario);

        try
        {
            await _context.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch (DbUpdateException ex)
        {
            // Incluye DbUpdateConcurrencyException (Version) y la violación del índice único de la bitácora.
            _logger.LogWarning(ex, "Decisión concurrente rechazada para la cotización {Id}", cmd.CotizacionId);
            await tx.RollbackAsync();
            _context.ChangeTracker.Clear();
            return new ResultadoRevision(CodigoRevision.Conflicto, MensajeConflicto, solicitud.Id);
        }

        // Correos fuera de la transacción: un fallo SMTP no revierte la decisión.
        await _notificaciones.EnviarCorreosAsync(decision, solicitud, cotizacion, comentario, orden?.Numero);

        var mensajeOk = decision switch
        {
            EstadosRevision.DecisionAprobada => $"Propuesta aprobada. Se generó la orden preliminar {orden!.Numero}.",
            EstadosRevision.DecisionModificacion => "Solicitud de modificación enviada. El cotizador ajustará la propuesta.",
            _ => "La propuesta fue rechazada y su motivo quedó registrado."
        };

        return new ResultadoRevision(CodigoRevision.Ok, mensajeOk, solicitud.Id, orden?.Id);
    }

    // =====================================================================
    // AUXILIARES
    // =====================================================================

    private async Task<bool> EsPropietarioAsync(SolicitudCotizacion solicitud, string usuarioId)
    {
        var correo = await _context.Users
            .Where(u => u.Id == usuarioId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync();

        return !string.IsNullOrWhiteSpace(correo)
               && string.Equals(correo, solicitud.Correo, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Propuesta visual: la vinculada a la cotización, o el diseño favorito, o el más reciente.</summary>
    private static DisenoIA? ResolverDiseno(SolicitudCotizacion solicitud, Cotizacion cotizacion)
    {
        if (cotizacion.DisenoIAId.HasValue)
        {
            var vinculado = solicitud.DisenosIA.FirstOrDefault(d => d.Id == cotizacion.DisenoIAId.Value);
            if (vinculado != null) return vinculado;
        }

        return solicitud.DisenosIA
            .OrderByDescending(d => d.EsFavorito)
            .ThenByDescending(d => d.FechaGeneracion)
            .FirstOrDefault();
    }

    private static string CrearSnapshot(Cotizacion c, SolicitudCotizacion s, DisenoIA? d) =>
        JsonSerializer.Serialize(new
        {
            CotizacionId = c.Id,
            c.CostoMateriales,
            c.ManoObra,
            c.CostoInstalacion,
            c.OtrosCostos,
            c.Subtotal,
            c.PorcentajeImpuesto,
            c.Impuesto,
            c.Descuento,
            c.Total,
            c.DetalleTecnico,
            c.Condiciones,
            FechaVencimiento = c.FechaVencimiento.ToString("yyyy-MM-dd"),
            Producto = s.Producto?.Nombre ?? s.TipoProducto,
            Material = s.Material?.Nombre,
            s.Ancho,
            s.Alto,
            s.Profundidad,
            s.UnidadMedida,
            s.Cantidad,
            DisenoIAId = d?.Id,
            DisenoImagen = d?.RutaImagenGenerada
        });

    private static string Sha256(string texto) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(texto))).ToLowerInvariant();

    private static string? Recortar(string? texto, int max) =>
        texto == null ? null : texto.Length <= max ? texto : texto[..max];
}

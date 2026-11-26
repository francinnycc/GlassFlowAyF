using GlassFlowAyF.Models.ViewModels;

namespace GlassFlowAyF.Services;

public enum CodigoRevision
{
    Ok,
    NoEncontrado,
    Prohibido,
    Invalido,
    Conflicto
}

/// <summary>Datos de la decisión enviados por el cliente más los datos de auditoría del request.</summary>
public record ComandoRevision(
    int CotizacionId,
    Guid Version,
    string? Comentario,
    bool AceptoTerminos,
    string UsuarioId,
    string? DireccionIp,
    string? UserAgent);

public record ResultadoRevision(CodigoRevision Codigo, string? Mensaje = null, int SolicitudId = 0, int? OrdenId = null)
{
    public bool Ok => Codigo == CodigoRevision.Ok;
}

public record ResultadoConsulta(CodigoRevision Codigo, RevisionPropuestaViewModel? Modelo = null);

public interface IRevisionClienteService
{
    /// <summary>
    /// Panel de revisión de una solicitud. Si <paramref name="cotizacionId"/> es null se toma la cotización más reciente.
    /// Modelo.Cotizacion == null indica que la propuesta aún no existe (Empty State).
    /// </summary>
    Task<ResultadoConsulta> ObtenerRevisionAsync(int solicitudId, string usuarioId, bool esAdministrador, int? cotizacionId = null);

    Task<ResultadoRevision> AprobarAsync(ComandoRevision comando);

    Task<ResultadoRevision> SolicitarModificacionAsync(ComandoRevision comando);

    Task<ResultadoRevision> RechazarAsync(ComandoRevision comando);
}

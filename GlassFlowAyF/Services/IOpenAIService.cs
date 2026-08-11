using GlassFlowAyF.Models;

namespace GlassFlowAyF.Services
{
    public interface IOpenAIService
    {
        string ModeloTexto { get; }

        string ModeloImagen { get; }


        Task<string> GenerarRecomendacionAsync(
            SolicitudCotizacion solicitud,
            SolicitudFotografia? fotografiaBase,
            string estilo,
            string colorPerfil,
            string ambiente,
            string? preferencias,
            CancellationToken cancellationToken =
                default);


        Task<byte[]> GenerarDisenoVisualAsync(
            SolicitudCotizacion solicitud,
            SolicitudFotografia? fotografiaBase,
            string estilo,
            string colorPerfil,
            string ambiente,
            string? preferencias,
            CancellationToken cancellationToken =
                default);
    }
}
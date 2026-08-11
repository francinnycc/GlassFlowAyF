using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using GlassFlowAyF.Models;

namespace GlassFlowAyF.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public string ModeloTexto { get; }

        public string ModeloImagen { get; }


        public OpenAIService(
            HttpClient httpClient,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _environment = environment;

            ModeloTexto =
                _configuration["OpenAI:TextModel"]
                ?? "gpt-5-mini";

            ModeloImagen =
                _configuration["OpenAI:ImageModel"]
                ?? "gpt-image-1";
        }


        // =====================================================
        // RECOMENDACIÓN TEXTUAL
        // =====================================================

        public async Task<string>
            GenerarRecomendacionAsync(
                SolicitudCotizacion solicitud,
                SolicitudFotografia? fotografiaBase,
                string estilo,
                string colorPerfil,
                string ambiente,
                string? preferencias,
                CancellationToken cancellationToken =
                    default)
        {
            ConfigurarAutorizacion();


            var contenido =
                new List<object>
                {
                    new
                    {
                        type = "input_text",

                        text =
                            ConstruirPromptRecomendacion(
                                solicitud,
                                estilo,
                                colorPerfil,
                                ambiente,
                                preferencias)
                    }
                };


            var imagenDataUrl =
                await ObtenerImagenDataUrlAsync(
                    fotografiaBase,
                    cancellationToken);


            if (!string.IsNullOrWhiteSpace(
                imagenDataUrl))
            {
                contenido.Add(
                    new
                    {
                        type = "input_image",

                        image_url =
                            imagenDataUrl,

                        detail = "high"
                    });
            }


            var request =
                new
                {
                    model =
                        ModeloTexto,

                    store =
                        false,

                    instructions =
                        "Eres un asistente de diseño para GlassFlow A&F, " +
                        "empresa de soluciones en vidrio y aluminio. " +
                        "Responde en español claro y profesional. " +
                        "Analiza estética, funcionalidad y compatibilidad visual. " +
                        "Nunca presentes la recomendación como cálculo estructural, " +
                        "certificación técnica ni garantía de seguridad. " +
                        "Indica de forma breve que medidas, espesores, fijaciones " +
                        "y requisitos de seguridad deben ser validados por un técnico. " +
                        "Organiza la respuesta con: Concepto recomendado, " +
                        "Materiales/acabados, Razones de diseño, Alternativa " +
                        "y Validación técnica.",

                    input =
                        new[]
                        {
                            new
                            {
                                role = "user",

                                content =
                                    contenido
                            }
                        }
                };


            using var response =
                await _httpClient
                    .PostAsJsonAsync(
                        "v1/responses",
                        request,
                        cancellationToken);


            var json =
                await response.Content
                    .ReadAsStringAsync(
                        cancellationToken);


            if (!response.IsSuccessStatusCode)
            {
                throw CrearExcepcionApi(
                    json);
            }


            using var document =
                JsonDocument.Parse(
                    json);


            var texto =
                ExtraerTexto(
                    document.RootElement);


            if (string.IsNullOrWhiteSpace(
                texto))
            {
                throw new InvalidOperationException(
                    "La IA no devolvió una recomendación de diseño.");
            }


            return texto.Trim();
        }


        // =====================================================
        // DISEÑO VISUAL
        // =====================================================

        public async Task<byte[]>
            GenerarDisenoVisualAsync(
                SolicitudCotizacion solicitud,
                SolicitudFotografia? fotografiaBase,
                string estilo,
                string colorPerfil,
                string ambiente,
                string? preferencias,
                CancellationToken cancellationToken =
                    default)
        {
            ConfigurarAutorizacion();


            var contenido =
                new List<object>
                {
                    new
                    {
                        type =
                            "input_text",

                        text =
                            ConstruirPromptImagen(
                                solicitud,
                                estilo,
                                colorPerfil,
                                ambiente,
                                preferencias,
                                fotografiaBase != null)
                    }
                };


            var imagenDataUrl =
                await ObtenerImagenDataUrlAsync(
                    fotografiaBase,
                    cancellationToken);


            if (!string.IsNullOrWhiteSpace(
                imagenDataUrl))
            {
                contenido.Add(
                    new
                    {
                        type =
                            "input_image",

                        image_url =
                            imagenDataUrl,

                        detail =
                            "high"
                    });
            }


            var herramientaImagen =
                new Dictionary<string, object>
                {
                    ["type"] =
                        "image_generation",

                    ["model"] =
                        ModeloImagen,

                    ["action"] =
                        fotografiaBase != null
                            ? "edit"
                            : "generate",

                    ["quality"] =
                        "medium",

                    ["size"] =
                        "1536x1024",

                    ["output_format"] =
                        "jpeg"
                };


            if (fotografiaBase != null)
            {
                herramientaImagen[
                    "input_fidelity"] =
                        "high";
            }


            var request =
                new
                {
                    model =
                        ModeloTexto,

                    store =
                        false,

                    input =
                        new[]
                        {
                            new
                            {
                                role =
                                    "user",

                                content =
                                    contenido
                            }
                        },

                    tools =
                        new object[]
                        {
                            herramientaImagen
                        }
                };


            using var response =
                await _httpClient
                    .PostAsJsonAsync(
                        "v1/responses",
                        request,
                        cancellationToken);


            var json =
                await response.Content
                    .ReadAsStringAsync(
                        cancellationToken);


            if (!response.IsSuccessStatusCode)
            {
                throw CrearExcepcionApi(
                    json);
            }


            using var document =
                JsonDocument.Parse(
                    json);


            var base64 =
                ExtraerImagenBase64(
                    document.RootElement);


            if (string.IsNullOrWhiteSpace(
                base64))
            {
                throw new InvalidOperationException(
                    "La IA no generó una imagen. " +
                    "Intente nuevamente con otra fotografía o preferencias.");
            }


            return Convert.FromBase64String(
                base64);
        }


        // =====================================================
        // AUTORIZACIÓN
        // =====================================================

        private void ConfigurarAutorizacion()
        {
            var apiKey =
                _configuration[
                    "OpenAI:ApiKey"];


            if (string.IsNullOrWhiteSpace(
                apiKey))
            {
                throw new InvalidOperationException(
                    "No se encontró OpenAI:ApiKey. " +
                    "Configure la clave mediante User Secrets.");
            }


            _httpClient
                .DefaultRequestHeaders
                .Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        apiKey);
        }


        // =====================================================
        // CARGAR FOTOGRAFÍA LOCAL COMO BASE64
        // =====================================================

        private async Task<string?>
            ObtenerImagenDataUrlAsync(
                SolicitudFotografia? fotografia,
                CancellationToken cancellationToken)
        {
            if (fotografia == null ||
                string.IsNullOrWhiteSpace(
                    fotografia.RutaArchivo))
            {
                return null;
            }


            var rutaRelativa =
                fotografia.RutaArchivo
                    .TrimStart('/')
                    .Replace(
                        '/',
                        Path.DirectorySeparatorChar);


            var rutaFisica =
                Path.Combine(
                    _environment.WebRootPath,
                    rutaRelativa);


            if (!File.Exists(
                rutaFisica))
            {
                return null;
            }


            var bytes =
                await File.ReadAllBytesAsync(
                    rutaFisica,
                    cancellationToken);


            var mimeType =
                ObtenerMimeType(
                    Path.GetExtension(
                        rutaFisica));


            return
                $"data:{mimeType};base64," +
                $"{Convert.ToBase64String(bytes)}";
        }


        // =====================================================
        // PROMPT DE RECOMENDACIÓN
        // =====================================================

        private static string
            ConstruirPromptRecomendacion(
                SolicitudCotizacion solicitud,
                string estilo,
                string colorPerfil,
                string ambiente,
                string? preferencias)
        {
            return $"""
                Analiza este proyecto de GlassFlow A&F y genera recomendaciones de diseño orientativas.

                Producto: {solicitud.Producto?.Nombre ?? solicitud.TipoProducto}
                Categoría: {solicitud.Producto?.Categoria ?? "No indicada"}
                Material actual: {solicitud.Material?.Nombre ?? "No seleccionado"}
                Tipo de material: {solicitud.Material?.Tipo ?? "No indicado"}
                Color actual: {solicitud.Material?.Color ?? "No indicado"}
                Perfil actual: {solicitud.Material?.Perfil ?? "No indicado"}
                Acabado actual: {solicitud.Material?.Acabado ?? "No indicado"}
                Grosor registrado: {solicitud.Material?.Grosor?.ToString("0.##") ?? "No indicado"} mm

                Medidas aproximadas:
                Ancho: {solicitud.Ancho?.ToString("0.00") ?? "?"} m
                Alto: {solicitud.Alto?.ToString("0.00") ?? "?"} m
                Profundidad: {solicitud.Profundidad?.ToString("0.00") ?? "No aplica"} m

                Cantidad: {solicitud.Cantidad}

                Descripción del cliente:
                {solicitud.Descripcion}

                Observaciones:
                {solicitud.Observaciones ?? "Ninguna"}

                Requiere instalación:
                {(solicitud.RequiereInstalacion ? "Sí" : "No")}

                Preferencias visuales nuevas:

                Estilo:
                {estilo}

                Color de perfilería:
                {colorPerfil}

                Ambiente deseado:
                {ambiente}

                Preferencias adicionales:
                {preferencias ?? "Ninguna"}

                Si se adjuntó una fotografía, analiza el espacio visible
                y adapta la recomendación a ese espacio.

                No inventes medidas a partir de la fotografía.
                """;
        }


        // =====================================================
        // PROMPT DE GENERACIÓN VISUAL
        // =====================================================

        private static string
            ConstruirPromptImagen(
                SolicitudCotizacion solicitud,
                string estilo,
                string colorPerfil,
                string ambiente,
                string? preferencias,
                bool tieneFotografia)
        {
            var accion =
                tieneFotografia
                    ? "Edita la fotografía proporcionada conservando " +
                      "la arquitectura, perspectiva, iluminación general " +
                      "y distribución del espacio. Integra visualmente " +
                      "la solución GlassFlow solicitada de forma realista."
                    : "Genera un concepto arquitectónico fotorrealista " +
                      "que represente la solución GlassFlow solicitada " +
                      "en un espacio residencial o comercial coherente.";


            return $"""
                {accion}

                Proyecto:
                {solicitud.Producto?.Nombre ?? solicitud.TipoProducto}.

                Material preferido:
                {solicitud.Material?.Nombre ?? "vidrio adecuado al proyecto"}.

                Estilo visual:
                {estilo}.

                Perfilería o marco:
                {colorPerfil}.

                Ambiente:
                {ambiente}.

                Preferencias adicionales:
                {preferencias ?? "ninguna"}.

                La propuesta debe verse fotorrealista,
                elegante y técnicamente plausible como concepto visual.

                No agregues texto, logotipos, medidas,
                marcas de agua ni personas.

                No cambies elementos del espacio que no sean necesarios
                para visualizar el producto.

                Mantén proporciones visuales razonables,
                pero no presentes la imagen como plano técnico
                ni como diseño estructural aprobado.
                """;
        }


        // =====================================================
        // EXTRAER TEXTO
        // =====================================================

        private static string ExtraerTexto(
            JsonElement root)
        {
            if (!root.TryGetProperty(
                    "output",
                    out var output) ||
                output.ValueKind !=
                    JsonValueKind.Array)
            {
                return string.Empty;
            }


            foreach (var item
                in output.EnumerateArray())
            {
                if (!item.TryGetProperty(
                        "type",
                        out var tipo) ||
                    tipo.GetString() !=
                        "message")
                {
                    continue;
                }


                if (!item.TryGetProperty(
                        "content",
                        out var content) ||
                    content.ValueKind !=
                        JsonValueKind.Array)
                {
                    continue;
                }


                foreach (var parte
                    in content.EnumerateArray())
                {
                    if (parte.TryGetProperty(
                            "type",
                            out var tipoParte) &&
                        tipoParte.GetString() ==
                            "output_text" &&
                        parte.TryGetProperty(
                            "text",
                            out var texto))
                    {
                        return texto.GetString()
                            ?? string.Empty;
                    }
                }
            }


            return string.Empty;
        }


        // =====================================================
        // EXTRAER IMAGEN
        // =====================================================

        private static string
            ExtraerImagenBase64(
                JsonElement root)
        {
            if (!root.TryGetProperty(
                    "output",
                    out var output) ||
                output.ValueKind !=
                    JsonValueKind.Array)
            {
                return string.Empty;
            }


            foreach (var item
                in output.EnumerateArray())
            {
                if (!item.TryGetProperty(
                        "type",
                        out var tipo) ||
                    tipo.GetString() !=
                        "image_generation_call")
                {
                    continue;
                }


                if (item.TryGetProperty(
                    "result",
                    out var result))
                {
                    return result.GetString()
                        ?? string.Empty;
                }
            }


            return string.Empty;
        }


        // =====================================================
        // ERRORES DE API
        // =====================================================

        private static InvalidOperationException
            CrearExcepcionApi(
                string json)
        {
            try
            {
                using var document =
                    JsonDocument.Parse(
                        json);


                if (document.RootElement
                    .TryGetProperty(
                        "error",
                        out var error) &&
                    error.TryGetProperty(
                        "message",
                        out var message))
                {
                    return new InvalidOperationException(
                        $"OpenAI: {message.GetString()}");
                }
            }
            catch (JsonException)
            {
                // Mensaje genérico abajo.
            }


            return new InvalidOperationException(
                "No fue posible completar la solicitud de IA. " +
                "Revise la configuración de OpenAI e intente nuevamente.");
        }


        // =====================================================
        // MIME TYPE
        // =====================================================

        private static string ObtenerMimeType(
            string extension)
        {
            return extension
                .ToLowerInvariant() switch
            {
                ".png" =>
                    "image/png",

                ".webp" =>
                    "image/webp",

                ".jpeg" =>
                    "image/jpeg",

                ".jpg" =>
                    "image/jpeg",

                _ =>
                    "image/jpeg"
            };
        }
    }
}
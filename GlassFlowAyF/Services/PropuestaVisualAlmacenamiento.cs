namespace GlassFlowAyF.Services
{
    public record ArchivoGuardado(
        string RutaRelativa,
        string NombreOriginal,
        string TipoContenido,
        string Extension,
        long Tamano);

    public record ErrorArchivo(string Nombre, string Motivo);

    /// <summary>
    /// Punto único de almacenamiento de adjuntos del módulo 14.
    /// Hoy guarda en disco (App_Data, fuera de wwwroot para que solo se acceda
    /// mediante enlaces autorizados). Si luego se migra a un Cloud Storage,
    /// solo se reemplaza esta implementación.
    /// </summary>
    public interface IPropuestaVisualAlmacenamiento
    {
        Task<List<ErrorArchivo>> ValidarAsync(IEnumerable<IFormFile> archivos);
        Task<ArchivoGuardado> GuardarAsync(int propuestaId, IFormFile archivo, CancellationToken ct = default);
        void Eliminar(string rutaRelativa);
        string ObtenerRutaFisica(string rutaRelativa);
    }

    public class PropuestaVisualAlmacenamiento : IPropuestaVisualAlmacenamiento
    {
        public const long TamanoMaximoBytes = 15L * 1024 * 1024; // 15 MB

        private static readonly HashSet<string> ExtensionesPermitidas =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".pdf", ".dwg" };

        private readonly string _raiz;

        public PropuestaVisualAlmacenamiento(IWebHostEnvironment env)
        {
            _raiz = Path.GetFullPath(Path.Combine(env.ContentRootPath, "App_Data"));
        }

        public async Task<List<ErrorArchivo>> ValidarAsync(IEnumerable<IFormFile> archivos)
        {
            var errores = new List<ErrorArchivo>();

            foreach (var archivo in archivos.Where(a => a != null))
            {
                var extension = Path.GetExtension(archivo.FileName);

                if (!ExtensionesPermitidas.Contains(extension))
                {
                    errores.Add(new ErrorArchivo(archivo.FileName, "extensión no permitida"));
                    continue;
                }

                if (archivo.Length == 0)
                {
                    errores.Add(new ErrorArchivo(archivo.FileName, "el archivo está vacío"));
                    continue;
                }

                if (archivo.Length > TamanoMaximoBytes)
                {
                    errores.Add(new ErrorArchivo(archivo.FileName, "supera los 15 MB"));
                    continue;
                }

                if (!await FirmaValidaAsync(archivo, extension))
                {
                    errores.Add(new ErrorArchivo(archivo.FileName, "el contenido no coincide con la extensión"));
                }
            }

            return errores;
        }

        public async Task<ArchivoGuardado> GuardarAsync(
            int propuestaId, IFormFile archivo, CancellationToken ct = default)
        {
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            var carpetaRelativa = Path.Combine("propuestas-visuales", propuestaId.ToString());
            var carpeta = Path.Combine(_raiz, carpetaRelativa);
            Directory.CreateDirectory(carpeta);

            var nombre = $"{Guid.NewGuid():N}{extension}";
            var rutaFisica = Path.Combine(carpeta, nombre);

            await using (var destino = File.Create(rutaFisica))
            {
                await archivo.CopyToAsync(destino, ct);
            }

            var relativa = $"propuestas-visuales/{propuestaId}/{nombre}";

            return new ArchivoGuardado(
                relativa,
                Path.GetFileName(archivo.FileName),
                archivo.ContentType ?? "application/octet-stream",
                extension,
                archivo.Length);
        }

        public void Eliminar(string rutaRelativa)
        {
            try
            {
                var ruta = ObtenerRutaFisica(rutaRelativa);
                if (File.Exists(ruta)) File.Delete(ruta);
            }
            catch (IOException) { /* si el archivo está en uso, el registro igual se elimina */ }
            catch (UnauthorizedAccessException) { }
        }

        public string ObtenerRutaFisica(string rutaRelativa)
        {
            var completa = Path.GetFullPath(Path.Combine(_raiz, rutaRelativa.Replace('/', Path.DirectorySeparatorChar)));

            // Evita salir de App_Data (path traversal).
            if (!completa.StartsWith(_raiz, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Ruta de archivo no válida.");

            return completa;
        }

        // Verifica los primeros bytes para no confiar solo en la extensión.
        private static async Task<bool> FirmaValidaAsync(IFormFile archivo, string extension)
        {
            var cabecera = new byte[8];
            await using var stream = archivo.OpenReadStream();
            var leidos = await stream.ReadAsync(cabecera.AsMemory(0, cabecera.Length));
            if (leidos < 4) return false;

            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => cabecera[0] == 0xFF && cabecera[1] == 0xD8 && cabecera[2] == 0xFF,
                ".png" => cabecera[0] == 0x89 && cabecera[1] == 0x50 && cabecera[2] == 0x4E && cabecera[3] == 0x47,
                ".pdf" => cabecera[0] == 0x25 && cabecera[1] == 0x50 && cabecera[2] == 0x44 && cabecera[3] == 0x46,
                ".dwg" => cabecera[0] == 0x41 && cabecera[1] == 0x43, // "AC" (AC1015, AC1027...)
                _ => false
            };
        }
    }
}

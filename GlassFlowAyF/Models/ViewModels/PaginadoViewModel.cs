using System.Collections;

namespace GlassFlowAyF.ViewModels
{
    /// <summary>
    /// Resultado paginado. Implementa IEnumerable para que las vistas que ya declaran
    /// "@model IEnumerable&lt;T&gt;" sigan funcionando sin cambios.
    /// </summary>
    public class PaginadoViewModel<T> : IEnumerable<T>
    {
        public List<T> Registros { get; set; } = new();

        public PaginacionViewModel Paginacion { get; set; } = new();

        public IEnumerator<T> GetEnumerator() => Registros.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

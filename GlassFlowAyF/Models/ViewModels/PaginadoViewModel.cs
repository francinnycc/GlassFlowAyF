namespace GlassFlowAyF.ViewModels
{
    public class PaginadoViewModel<T>
    {
        public List<T> Registros { get; set; } = new();

        public PaginacionViewModel Paginacion { get; set; }
            = new();
    }
}
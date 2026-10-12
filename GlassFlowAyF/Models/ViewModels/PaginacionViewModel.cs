namespace GlassFlowAyF.ViewModels
{
    public class PaginacionViewModel
    {
        public int PaginaActual { get; set; }

        public int RegistrosPorPagina { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalPaginas =>
            (int)Math.Ceiling(
                TotalRegistros /
                (double)RegistrosPorPagina);

        public bool TieneAnterior =>
            PaginaActual > 1;

        public bool TieneSiguiente =>
            PaginaActual < TotalPaginas;
    }
}

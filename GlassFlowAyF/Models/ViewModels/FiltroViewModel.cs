namespace GlassFlowAyF.ViewModels
{
    /// <summary>
    /// Filtros comunes de las tablas del sistema. Se recibe con [FromQuery] y se
    /// entrega a la vista (ViewBag.Filtro) para pintar la barra de filtros.
    /// </summary>
    public class FiltroViewModel
    {
        public string? Buscar { get; set; }
        public string? Estado { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public int Pagina { get; set; } = 1;
        private int _tamanoPagina = 10;

        // Limitado para evitar consultas enormes por URL manipulada.
        public int TamanoPagina
        {
            get => _tamanoPagina;
            set => _tamanoPagina = Math.Clamp(value, 5, 50);
        }

        // Configuración de la barra (la define cada controlador).
        public List<string> Estados { get; set; } = new();
        public string Placeholder { get; set; } = "Buscar...";
        public bool MostrarEstado { get; set; } = true;
        public bool MostrarFechas { get; set; } = true;

        public DateTime? HastaInclusivo => Hasta?.Date.AddDays(1);
    }
}

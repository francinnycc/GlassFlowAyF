using System.ComponentModel.DataAnnotations;

namespace GlassFlowAyF.Models
{
    public class DisenoIA
    {
        public int Id { get; set; }

        [Required]
        public int SolicitudCotizacionId { get; set; }

        public SolicitudCotizacion? SolicitudCotizacion { get; set; }

        public int? FotografiaBaseId { get; set; }

        public SolicitudFotografia? FotografiaBase { get; set; }

        [Required]
        [StringLength(60)]
        public string Estilo { get; set; } = string.Empty;

        [StringLength(60)]
        [Display(Name = "Color de perfilería")]
        public string? ColorPerfil { get; set; }

        [StringLength(80)]
        [Display(Name = "Ambiente deseado")]
        public string? Ambiente { get; set; }

        [StringLength(1000)]
        public string? Preferencias { get; set; }

        [StringLength(5000)]
        public string? Recomendacion { get; set; }

        [StringLength(5000)]
        public string? PromptGeneracion { get; set; }

        [StringLength(500)]
        [Display(Name = "Imagen generada")]
        public string? RutaImagenGenerada { get; set; }

        [StringLength(100)]
        public string? ModeloTexto { get; set; }

        [StringLength(100)]
        public string? ModeloImagen { get; set; }

        [Display(Name = "Diseño preferido")]
        public bool EsFavorito { get; set; }

        public DateTime FechaGeneracion { get; set; }
            = DateTime.Now;
    }
}
namespace GlassFlowAyF.Models
{
    public class ProductoMaterial
    {
        public int ProductoId { get; set; }

        public Producto? Producto { get; set; }

        public int MaterialId { get; set; }

        public Material? Material { get; set; }
    }
}
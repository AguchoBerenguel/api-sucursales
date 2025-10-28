namespace ApiSucursal.Modelos
{
    public class Inventario
    {
        public int Id_Inventario { get; set; }
        public int Id_Producto { get; set; }
        public int Talle { get; set; }
        public int Cantidad { get; set; }
        public ProductoDetalle? Producto { get; set; }
    }
}

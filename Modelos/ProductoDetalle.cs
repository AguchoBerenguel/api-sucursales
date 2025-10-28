namespace ApiSucursal.Modelos
{
     public class ProductoDetalle
    {
        public int Id_Producto { get; set; }
        public string Articulo_Local { get; set; } = string.Empty;
        public string Articulo_Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public decimal Precio_Unitario { get; set; }
        public decimal Precio_Vidriera { get; set; }
        public int Talle { get; set; }
        public int Cantidad { get; set; }
        public string Nombre_Proveedor { get; set; } = string.Empty;
    }
}

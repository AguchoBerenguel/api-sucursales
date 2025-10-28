using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ApiSucursal.Modelos;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ApiSucursal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        // 1. Variable privada para almacenar la configuración.
        private readonly IConfiguration _configuration;

        // 2. Este es el "Constructor". Se ejecuta automáticamente cuando se
        //    necesita el controlador. ASP.NET Core le "inyecta" la configuración.
        public StockController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // 3. Modificamos el método para que acepte el ID de la sucursal en la URL.
        //    La URL ahora será: /api/stock/sucursalA  o  /api/stock/sucursalB
        [HttpGet("{idSucursal}")]
        public IActionResult GetStock(string idSucursal, [FromQuery] string articulo = "")
        {
            string connectionString;

            if (idSucursal.Equals("sucursalA", StringComparison.OrdinalIgnoreCase))
            {
                connectionString = _configuration.GetConnectionString("SucursalA");
            }
            else if (idSucursal.Equals("sucursalB", StringComparison.OrdinalIgnoreCase))
            {
                connectionString = _configuration.GetConnectionString("SucursalB");
            }
            else
            {
                // Si la URL no es válida, devolvemos un error claro.
                return BadRequest("El ID de sucursal no es válido. Use 'sucursalA' o 'sucursalB'.");
            }

            // 5. El resto de tu código para acceder a la base de datos permanece
            //    casi igual, pero ahora usa la variable 'connectionString' que
            //    seleccionamos dinámicamente.
            var lista = new List<ProductoDetalle>();

            try // Es buena práctica envolver el acceso a la base de datos en un try-catch
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT p.id_producto, p.articulo_local, p.articulo_marca, p.modelo, p.precio_unitario, p.precio_vidriera, i.talle, i.cantidad, pr.nombre_proveedor AS NombreProveedor FROM " +
                                   "productos AS p JOIN inventario AS i ON p.id_producto = i.id_producto JOIN proveedor AS pr ON " +
                                   "p.id_proveedor = pr.id_proveedor";

                    if (!string.IsNullOrEmpty(articulo))
                        query += " WHERE p.articulo_local LIKE @articulo";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(articulo))
                            cmd.Parameters.AddWithValue("@articulo", $"%{articulo}%");

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new ProductoDetalle
                                {
                                    Id_Producto = reader.GetInt32("Id_Producto"),
                                    Articulo_Local = reader.GetString("Articulo_Local"),
                                    Articulo_Marca = reader.GetString("Articulo_Marca"),
                                    Modelo = reader.GetString("Modelo"),
                                    Precio_Unitario = reader.GetDecimal("Precio_Unitario"),
                                    Precio_Vidriera = reader.GetDecimal("Precio_Vidriera"),
                                    Talle = reader.GetInt32("Talle"),
                                    Cantidad = reader.GetInt32("Cantidad"),
                                    Nombre_Proveedor = reader.GetString("NombreProveedor")
                                });
                            }
                        }
                    }
                }
                return Ok(lista);
            }
            catch (Exception ex)
            {
                // Si algo falla (ej: la base de datos está offline), devolvemos un error 500.
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
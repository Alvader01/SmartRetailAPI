using System.Text.Json.Serialization;

namespace SmartRetailApi.Models
{
    /// <summary>
    /// Representa el detalle de una venta, incluyendo el producto vendido,
    /// la cantidad y el subtotal correspondiente.
    /// La clave primaria está compuesta por VentaId, ProductoId y TiendaId.
    /// </summary>
    public class DetalleVenta
    {
        /// <summary>
        /// Identificador de la venta a la que pertenece este detalle.
        /// Parte de la clave primaria compuesta.
        /// </summary>
        public Guid VentaId { get; set; }

        /// <summary>
        /// Identificador del producto vendido.
        /// Parte de la clave primaria compuesta.
        /// </summary>
        public Guid ProductoId { get; set; }

        /// <summary>
        /// Identificador de la tienda donde se realizó la venta.
        /// Parte de la clave primaria compuesta.
        /// </summary>
        public string TiendaId { get; set; } = null!;

        /// <summary>
        /// Cantidad de productos vendidos.
        /// </summary>
        public int Cantidad { get; set; }

        /// <summary>
        /// Subtotal correspondiente a esta cantidad de producto.
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Referencia a la venta completa a la que pertenece este detalle.
        /// Se ignora en la serialización JSON para evitar ciclos.
        /// </summary>
        public Venta? Venta { get; set; }

        /// <summary>
        /// Referencia al producto vendido en este detalle.
        /// </summary>
        public Producto? Producto { get; set; }
    }
}

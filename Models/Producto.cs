using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartRetailApi.Models
{
    /// <summary>
    /// Representa un producto disponible en la tienda, con su precio y stock.
    /// </summary>
    public class Producto
    {
        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        public int ProductoId { get; set; }

        /// <summary>
        /// Identificador de la tienda que vende el producto.
        /// </summary>
        public string TiendaId { get; set; } = null!;

        /// <summary>
        /// Nombre descriptivo del producto.
        /// </summary>
        public string Nombre { get; set; } = null!;

        /// <summary>
        /// Precio unitario del producto.
        /// </summary>
        public decimal Precio { get; set; }

        /// <summary>
        /// Cantidad disponible en stock.
        /// </summary>
        public int Stock { get; set; }
    }
}
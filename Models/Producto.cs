using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartRetailApi.Models
{
    /// <summary>
    /// Representa un producto disponible en la tienda, con su precio y stock.
    /// La clave primaria está compuesta por ProductoId y TiendaId.
    /// </summary>
    public class Producto
    {
        /// <summary>
        /// Identificador único del producto dentro de una tienda.
        /// Parte de la clave primaria compuesta.
        /// </summary>
        public Guid ProductoId { get; set; }

        /// <summary>
        /// Identificador de la tienda que vende el producto.
        /// Parte de la clave primaria compuesta.
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

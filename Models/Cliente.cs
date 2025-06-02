namespace SmartRetailApi.Models
{
    /// <summary>
    /// Representa un cliente de la tienda con información básica de contacto.
    /// La clave primaria está compuesta por ClienteId y TiendaId para soportar múltiples tiendas.
    /// </summary>
    public class Cliente
    {
        /// <summary>
        /// Identificador único del cliente dentro de una tienda.
        /// </summary>
        public Guid ClienteId { get; set; }

        /// <summary>
        /// Identificador de la tienda a la que pertenece el cliente.
        /// Parte de la clave primaria compuesta.
        /// </summary>
        public string TiendaId { get; set; } = null!;

        /// <summary>
        /// Nombre completo del cliente.
        /// </summary>
        public string Nombre { get; set; } = null!;

        /// <summary>
        /// Correo electrónico del cliente (opcional).
        /// </summary>
        public string? Correo { get; set; }

        /// <summary>
        /// Teléfono de contacto del cliente (opcional).
        /// </summary>
        public string? Telefono { get; set; }

        /// <summary>
        /// Colección de ventas asociadas a este cliente.
        /// Relación uno a muchos.
        /// </summary>
        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    }
}

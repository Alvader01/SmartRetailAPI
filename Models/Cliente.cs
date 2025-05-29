namespace SmartRetailApi.Models
{
    /// <summary>
    /// Representa un cliente de la tienda con información básica de contacto.
    /// </summary>
    public class Cliente
    {
        /// <summary>
        /// Identificador único del cliente.
        /// </summary>
        public int ClienteId { get; set; }

        /// <summary>
        /// Identificador de la tienda a la que pertenece el cliente.
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
    }
}
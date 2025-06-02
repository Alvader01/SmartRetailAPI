using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartRetailApi.Models
{
    /// <summary>
    /// Representa una venta realizada en la tienda, incluyendo fecha, cliente y detalles.
    /// La clave primaria está compuesta por VentaId y TiendaId.
    /// </summary>
    public class Venta
    {
        /// <summary>
        /// Identificador único de la venta dentro de una tienda.
        /// Parte de la clave primaria compuesta.
        /// </summary>
        public Guid VentaId { get; set; }

        /// <summary>
        /// Identificador de la tienda donde se realizó la venta.
        /// Parte de la clave primaria compuesta.
        /// </summary>
        public string TiendaId { get; set; } = null!;

        private DateTime _fecha;

        /// <summary>
        /// Fecha y hora de la venta, siempre en UTC.
        /// </summary>
        public DateTime Fecha
        {
            get => _fecha;
            set => _fecha = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        /// <summary>
        /// Importe total de la venta.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Identificador del cliente que realizó la compra.
        /// </summary>
        public Guid ClienteId { get; set; }

        /// <summary>
        /// Referencia al cliente que realizó la venta.
        /// La relación debe mapearse considerando ClienteId y TiendaId.
        /// </summary>
        public Cliente? Cliente { get; set; }

        /// <summary>
        /// Colección de detalles que componen la venta.
        /// </summary>
        public ICollection<DetalleVenta>? DetallesVenta { get; set; }
    }
}

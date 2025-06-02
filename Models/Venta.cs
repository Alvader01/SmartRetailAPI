using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartRetailApi.Models
{
    /// <summary>
    /// Representa una venta realizada en la tienda, incluyendo fecha, cliente y detalles.
    /// </summary>
    public class Venta
    {
        /// <summary>
        /// Identificador único de la venta.
        /// </summary>
        public int VentaId { get; set; }

        /// <summary>
        /// Identificador de la tienda donde se realizó la venta.
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
        public int ClienteId { get; set; }

        /// <summary>
        /// Referencia al cliente que realizó la venta.
        /// </summary>
        public Cliente? Cliente { get; set; }

        /// <summary>
        /// Colección de detalles que componen la venta.
        /// </summary>
        public ICollection<DetalleVenta>? DetallesVenta { get; set; }
    }
}
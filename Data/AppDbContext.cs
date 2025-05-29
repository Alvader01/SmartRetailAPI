using Microsoft.EntityFrameworkCore;
using SmartRetailApi.Models;

namespace SmartRetailApi.Data
{
    /// <summary>
    /// Contexto de base de datos que gestiona las entidades de la aplicación SmartRetail.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Constructor que recibe las opciones de configuración del contexto.
        /// </summary>
        /// <param name="options">Opciones para configurar el contexto de base de datos.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Representa la tabla Productos en la base de datos.
        /// </summary>
        public DbSet<Producto> Productos { get; set; }

        /// <summary>
        /// Representa la tabla Ventas en la base de datos.
        /// </summary>
        public DbSet<Venta> Ventas { get; set; }

        /// <summary>
        /// Representa la tabla DetallesVenta en la base de datos.
        /// </summary>
        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        /// <summary>
        /// Representa la tabla Clientes en la base de datos.
        /// </summary>
        public DbSet<Cliente> Clientes { get; set; }

        /// <summary>
        /// Configura el esquema y las relaciones de las tablas usando Fluent API.
        /// </summary>
        /// <param name="modelBuilder">Constructor de modelo para definir las entidades y relaciones.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de la entidad Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("producto");  // Nombre de tabla en la BD
                entity.HasKey(e => new { e.ProductoId, e.TiendaId }); // Clave primaria compuesta

                entity.Property(e => e.ProductoId).HasColumnName("producto_id");
                entity.Property(e => e.TiendaId).HasColumnName("tiendaid");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Precio).HasColumnName("precio");
                entity.Property(e => e.Stock).HasColumnName("stock");
            });

            // Configuración de la entidad Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("cliente");
                entity.HasKey(e => new { e.ClienteId, e.TiendaId }); // Clave primaria compuesta

                entity.Property(e => e.ClienteId).HasColumnName("cliente_id");
                entity.Property(e => e.TiendaId).HasColumnName("tiendaid");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Correo).HasColumnName("correo");
                entity.Property(e => e.Telefono).HasColumnName("telefono");
            });

            // Configuración de la entidad Venta
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.ToTable("venta");
                entity.HasKey(e => new { e.VentaId, e.TiendaId }); // Clave primaria compuesta

                entity.Property(e => e.VentaId).HasColumnName("venta_id");
                entity.Property(e => e.TiendaId).HasColumnName("tiendaid");
                entity.Property(e => e.Fecha).HasColumnName("fecha");
                entity.Property(e => e.Total).HasColumnName("total");
                entity.Property(e => e.ClienteId).HasColumnName("cliente_id");

                // Relación entre Venta y Cliente mediante clave foránea compuesta
                entity.HasOne(e => e.Cliente)
                      .WithMany()
                      .HasForeignKey(e => new { e.ClienteId, e.TiendaId });
            });

            // Configuración de la entidad DetalleVenta
            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.ToTable("detalle_venta");
                entity.HasKey(e => new { e.VentaId, e.ProductoId, e.TiendaId }); // Clave primaria compuesta

                entity.Property(e => e.VentaId).HasColumnName("venta_id");
                entity.Property(e => e.ProductoId).HasColumnName("producto_id");
                entity.Property(e => e.TiendaId).HasColumnName("tiendaid");
                entity.Property(e => e.Cantidad).HasColumnName("cantidad");
                entity.Property(e => e.Subtotal).HasColumnName("subtotal");

                // Relación entre DetalleVenta y Venta
                entity.HasOne(e => e.Venta)
                      .WithMany(v => v.DetallesVenta)
                      .HasForeignKey(e => new { e.VentaId, e.TiendaId });

                // Relación entre DetalleVenta y Producto
                entity.HasOne(e => e.Producto)
                      .WithMany()
                      .HasForeignKey(e => new { e.ProductoId, e.TiendaId });
            });
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SmartRetailApi.Models;

namespace SmartRetailApi.Data
{
    /// <summary>
    /// Contexto de base de datos que gestiona las entidades de la aplicación SmartRetail.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets que representan las tablas en la base de datos
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        /// <summary>
        /// Configura el esquema y las relaciones de las tablas usando Fluent API.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ====== Producto ======
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("producto");

                entity.HasKey(e => new { e.ProductoId, e.TiendaId }); // Clave primaria compuesta

                entity.Property(e => e.ProductoId)
                      .HasColumnName("producto_id");
                // GUIDs asignados manualmente en la aplicación

                entity.Property(e => e.TiendaId).HasColumnName("tiendaid");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Precio).HasColumnName("precio");
                entity.Property(e => e.Stock).HasColumnName("stock");
            });

            // ====== Cliente ======
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

            // ====== Venta ======
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.ToTable("venta");

                entity.HasKey(e => new { e.VentaId, e.TiendaId }); // Clave primaria compuesta

                entity.Property(e => e.VentaId).HasColumnName("venta_id");
                entity.Property(e => e.TiendaId).HasColumnName("tiendaid");
                entity.Property(e => e.Fecha).HasColumnName("fecha");
                entity.Property(e => e.Total).HasColumnName("total");
                entity.Property(e => e.ClienteId).HasColumnName("cliente_id");

                // Relación con Cliente (clave foránea compuesta)
                entity.HasOne(e => e.Cliente)
                      .WithMany(c => c.Ventas)
                      .HasForeignKey(e => new { e.ClienteId, e.TiendaId })
                      .OnDelete(DeleteBehavior.Restrict);
                // Restrict evita que se borren ventas si se borra cliente
            });

            // ====== DetalleVenta ======
            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.ToTable("detalle_venta");

                entity.HasKey(e => new { e.VentaId, e.ProductoId, e.TiendaId }); // Clave primaria compuesta

                entity.Property(e => e.VentaId).HasColumnName("venta_id");
                entity.Property(e => e.ProductoId).HasColumnName("producto_id");
                entity.Property(e => e.TiendaId).HasColumnName("tiendaid");
                entity.Property(e => e.Cantidad).HasColumnName("cantidad");
                entity.Property(e => e.Subtotal).HasColumnName("subtotal");

                // Relación con Venta (clave foránea compuesta)
                entity.HasOne(e => e.Venta)
                      .WithMany(v => v.DetallesVenta)
                      .HasForeignKey(e => new { e.VentaId, e.TiendaId })
                      .OnDelete(DeleteBehavior.Cascade);
                // Al borrar una venta, se borran sus detalles

                // Relación con Producto (clave foránea compuesta)
                entity.HasOne(e => e.Producto)
                      .WithMany()
                      .HasForeignKey(e => new { e.ProductoId, e.TiendaId })
                      .OnDelete(DeleteBehavior.Restrict);
                // No se permite borrar un producto si está en detalles de venta
            });
        }
    }
}

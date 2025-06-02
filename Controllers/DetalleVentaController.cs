using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRetailApi.Data;
using SmartRetailApi.Models;

[Authorize] // Requiere autenticación para todas las acciones
[ApiController] // Controlador API
[Route("api/[controller]")]
public class DetallesVentaController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Constructor que inyecta el contexto de la base de datos.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a DetallesVenta.</param>
    public DetallesVentaController(AppDbContext context) => _context = context;

    /// <summary>
    /// Obtiene todos los detalles de venta almacenados en la base de datos.
    /// </summary>
    /// <returns>Lista de objetos DetalleVenta.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DetalleVenta>>> Get() =>
        await _context.DetallesVenta.ToListAsync();

    /// <summary>
    /// Obtiene un detalle de venta específico por su clave compuesta: VentaId, ProductoId y TiendaId.
    /// </summary>
    /// <param name="ventaId">Identificador de la venta.</param>
    /// <param name="productoId">Identificador del producto.</param>
    /// <param name="tiendaId">Identificador de la tienda.</param>
    /// <returns>DetalleVenta encontrado o NotFound si no existe.</returns>
    [HttpGet("{ventaId}/{productoId}/{tiendaId}")]
    public async Task<ActionResult<DetalleVenta>> Get(int ventaId, int productoId, string tiendaId)
    {
        var detalle = await _context.DetallesVenta
            .FirstOrDefaultAsync(d => d.VentaId == ventaId && d.ProductoId == productoId && d.TiendaId == tiendaId);

        if (detalle == null)
        {
            return NotFound();
        }

        return detalle;
    }

    /// <summary>
    /// Crea un nuevo detalle de venta en la base de datos.
    /// </summary>
    /// <param name="detalle">Objeto DetalleVenta a insertar.</param>
    /// <returns>DetalleVenta creado con código HTTP 201 y ubicación.</returns>
    [HttpPost]
    public async Task<ActionResult<DetalleVenta>> Post(DetalleVenta detalle)
    {
        // Validar TiendaId obligatorio
        if (string.IsNullOrEmpty(detalle.TiendaId))
        {
            return BadRequest("TiendaId es obligatorio.");
        }

        // Verificar que no exista ya un detalle con la misma clave compuesta
        var existe = await _context.DetallesVenta.AnyAsync(d =>
            d.VentaId == detalle.VentaId &&
            d.ProductoId == detalle.ProductoId &&
            d.TiendaId == detalle.TiendaId);

        if (existe)
        {
            return Conflict("Ya existe un detalle de venta con los mismos IDs.");
        }

        _context.DetallesVenta.Add(detalle);
        await _context.SaveChangesAsync();

        // Devuelve 201 Created con la ruta completa a este recurso
        return CreatedAtAction(nameof(Get), new { ventaId = detalle.VentaId, productoId = detalle.ProductoId, tiendaId = detalle.TiendaId }, detalle);
    }
}

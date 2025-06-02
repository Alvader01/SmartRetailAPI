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
    public async Task<ActionResult> Post([FromBody] List<DetalleVenta> detalles)
    {
        if (detalles == null || detalles.Count == 0)
            return BadRequest("No se recibieron detalles de venta.");

        if (detalles.Any(d => string.IsNullOrEmpty(d.TiendaId)))
            return BadRequest("Todos los detalles deben tener TiendaId.");

        var claves = detalles.Select(d => new { d.VentaId, d.ProductoId, d.TiendaId }).ToList();

        var ventaIds = claves.Select(k => k.VentaId).Distinct().ToList();
        var productoIds = claves.Select(k => k.ProductoId).Distinct().ToList();
        var tiendaIds = claves.Select(k => k.TiendaId).Distinct().ToList();

        var existentes = await _context.DetallesVenta
            .Where(dv => ventaIds.Contains(dv.VentaId) &&
                         productoIds.Contains(dv.ProductoId) &&
                         tiendaIds.Contains(dv.TiendaId))
            .ToListAsync();

        var conflictos = existentes.Where(e => claves.Any(k =>
            k.VentaId == e.VentaId &&
            k.ProductoId == e.ProductoId &&
            k.TiendaId == e.TiendaId)).ToList();

        if (conflictos.Any())
        {
            return Conflict("Ya existen detalles de venta con algunas de las claves proporcionadas.");
        }

        _context.DetallesVenta.AddRange(detalles);
        await _context.SaveChangesAsync();

        return Ok(new { insertedCount = detalles.Count });
    }


}

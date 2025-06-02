using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRetailApi.Data;
using SmartRetailApi.Models;

[Authorize] // Requiere autenticación para todas las acciones del controlador
[ApiController] // Marca la clase como controlador API con funcionalidades automáticas
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
    /// Considera que esto puede devolver muchos datos si la base es grande.
    /// </summary>
    /// <returns>Lista de objetos DetalleVenta.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DetalleVenta>>> Get() =>
        await _context.DetallesVenta.ToListAsync();

    /// <summary>
    /// Obtiene un detalle de venta específico por su clave compuesta: VentaId, ProductoId y TiendaId.
    /// Esta combinación identifica unívocamente cada detalle de venta.
    /// </summary>
    /// <param name="ventaId">Identificador de la venta.</param>
    /// <param name="productoId">Identificador del producto.</param>
    /// <param name="tiendaId">Identificador de la tienda.</param>
    /// <returns>DetalleVenta encontrado o NotFound si no existe.</returns>
    [HttpGet("{ventaId}/{productoId}/{tiendaId}")]
    public async Task<ActionResult<DetalleVenta>> Get(Guid ventaId, Guid productoId, string tiendaId)
    {
        var detalle = await _context.DetallesVenta
            .FirstOrDefaultAsync(d => d.VentaId == ventaId && d.ProductoId == productoId && d.TiendaId == tiendaId);

        if (detalle == null)
        {
            return NotFound();
        }

        return Ok(detalle); // Retorna explícitamente con código 200 OK
    }

    /// <summary>
    /// Crea uno o varios detalles de venta nuevos en la base de datos.
    /// Se valida que la lista no esté vacía y que cada detalle tenga TiendaId definido.
    /// Además, se verifica que no existan detalles con la misma clave compuesta para evitar conflictos.
    /// </summary>
    /// <param name="detalles">Lista de objetos DetalleVenta a insertar.</param>
    /// <returns>Resultado con número de detalles insertados o error si la petición es inválida o hay conflictos.</returns>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] List<DetalleVenta> detalles)
    {
        if (detalles == null || detalles.Count == 0)
            return BadRequest("No se recibieron detalles de venta.");

        if (detalles.Any(d => string.IsNullOrEmpty(d.TiendaId)))
            return BadRequest("Todos los detalles deben tener TiendaId.");

        // Extrae las claves compuestas para comparar con la base de datos
        var claves = detalles.Select(d => new { d.VentaId, d.ProductoId, d.TiendaId }).ToList();

        var ventaIds = claves.Select(k => k.VentaId).Distinct().ToList();
        var productoIds = claves.Select(k => k.ProductoId).Distinct().ToList();
        var tiendaIds = claves.Select(k => k.TiendaId).Distinct().ToList();

        // Consulta existentes para evitar insertar duplicados
        var existentes = await _context.DetallesVenta
            .Where(dv => ventaIds.Contains(dv.VentaId) &&
                         productoIds.Contains(dv.ProductoId) &&
                         tiendaIds.Contains(dv.TiendaId))
            .ToListAsync();

        // Busca claves que ya existen para evitar conflictos
        var conflictos = existentes.Where(e => claves.Any(k =>
            k.VentaId == e.VentaId &&
            k.ProductoId == e.ProductoId &&
            k.TiendaId == e.TiendaId)).ToList();

        if (conflictos.Any())
        {
            // Retorna error 409 indicando que ya existen detalles con las claves proporcionadas
            return Conflict("Ya existen detalles de venta con algunas de las claves proporcionadas.");
        }

        _context.DetallesVenta.AddRange(detalles);
        await _context.SaveChangesAsync();

        // Retorna el número de registros insertados como confirmación
        return Ok(new { insertedCount = detalles.Count });
    }
}

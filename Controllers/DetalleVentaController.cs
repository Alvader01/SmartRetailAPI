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
    /// Inserta o actualiza (upsert) una lista de detalles de venta en la base de datos.
    /// Se valida que la lista no esté vacía y que cada detalle tenga TiendaId definido.
    /// Si un detalle ya existe (misma clave compuesta VentaId + ProductoId + TiendaId), se actualizan sus campos.
    /// </summary>
    /// <param name="detalles">Lista de objetos DetalleVenta a insertar o actualizar.</param>
    /// <returns>Resultado con número de detalles procesados o error si la petición es inválida.</returns>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] List<DetalleVenta> detalles)
    {
        if (detalles == null || detalles.Count == 0)
            return BadRequest("No se recibieron detalles de venta.");

        if (detalles.Any(d => string.IsNullOrEmpty(d.TiendaId)))
            return BadRequest("Todos los detalles deben tener TiendaId.");

        foreach (var detalle in detalles)
        {
            // Buscar detalle existente por clave compuesta
            var detalleExistente = await _context.DetallesVenta
                .FirstOrDefaultAsync(d => d.VentaId == detalle.VentaId &&
                                          d.ProductoId == detalle.ProductoId &&
                                          d.TiendaId == detalle.TiendaId);

            if (detalleExistente == null)
            {
                // Si no existe, agregar nuevo detalle
                _context.DetallesVenta.Add(detalle);
            }
            else
            {
                // Si existe, actualizar campos relevantes
                detalleExistente.Cantidad = detalle.Cantidad;
                detalleExistente.Subtotal = detalle.Subtotal;
                // No se actualizan referencias de navegación para evitar conflictos
            }
        }

        await _context.SaveChangesAsync();

        // Retorna un resultado con la cantidad de registros procesados (insertados + actualizados)
        return Ok(new { processedCount = detalles.Count });
    }
}

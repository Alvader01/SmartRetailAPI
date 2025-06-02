using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRetailApi.Data;
using SmartRetailApi.Models;

[Authorize] // Requiere autenticación para todas las acciones
[ApiController] // Indica que es un controlador API
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Constructor que inyecta el contexto de la base de datos.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a Ventas.</param>
    public VentasController(AppDbContext context) => _context = context;

    /// <summary>
    /// Obtiene todas las ventas junto con sus detalles asociados.
    /// Considerar paginación o filtros si la tabla es muy grande para mejorar rendimiento.
    /// </summary>
    /// <returns>Lista de objetos Venta con sus detalles.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Venta>>> Get() =>
        await _context.Ventas.Include(v => v.DetallesVenta).ToListAsync();

    /// <summary>
    /// Obtiene una venta específica por su VentaId y TiendaId.
    /// La combinación de VentaId y TiendaId es la clave única para identificar la venta.
    /// </summary>
    /// <param name="id">Identificador de la venta (VentaId).</param>
    /// <param name="tiendaId">Identificador de la tienda (TiendaId).</param>
    /// <returns>Venta con sus detalles o NotFound si no existe.</returns>
    [HttpGet("{id}/{tiendaId}")]
    public async Task<ActionResult<Venta>> Get(Guid id, string tiendaId)
    {
        var venta = await _context.Ventas
            .Include(v => v.DetallesVenta) // Incluye detalles relacionados
            .FirstOrDefaultAsync(v => v.VentaId == id && v.TiendaId == tiendaId);

        if (venta == null)
        {
            return NotFound();
        }

        return Ok(venta); // Devuelve código 200 OK explícitamente
    }

    /// <summary>
    /// Crea una o varias nuevas ventas en la base de datos.
    /// Valida que la lista no sea nula o vacía y que todas las ventas tengan TiendaId.
    /// </summary>
    /// <param name="ventas">Lista de objetos Venta a insertar.</param>
    /// <returns>Cantidad de ventas insertadas o error si la petición es inválida.</returns>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] List<Venta> ventas)
    {
        if (ventas == null || ventas.Count == 0)
            return BadRequest("No se recibieron ventas.");

        if (ventas.Any(v => string.IsNullOrEmpty(v.TiendaId)))
            return BadRequest("Todas las ventas deben tener TiendaId.");

        _context.Ventas.AddRange(ventas);
        await _context.SaveChangesAsync();

        // Retorna la cantidad de ventas insertadas para confirmar la operación
        return Ok(new { insertedCount = ventas.Count });
    }
}

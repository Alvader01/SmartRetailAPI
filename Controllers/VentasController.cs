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
    /// Verifica que no existan ventas con la misma clave compuesta para evitar duplicados.
    /// </summary>
    /// <param name="ventas">Lista de objetos Venta a insertar.</param>
    /// <returns>Cantidad de ventas insertadas o error si la petición es inválida o hay conflictos.</returns>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] List<Venta> ventas)
    {
        if (ventas == null || ventas.Count == 0)
            return BadRequest("No se recibieron ventas.");

        if (ventas.Any(v => string.IsNullOrEmpty(v.TiendaId)))
            return BadRequest("Todas las ventas deben tener TiendaId.");

        // Extrae las claves compuestas para comparar con la base de datos
        var claves = ventas.Select(v => new { v.VentaId, v.TiendaId }).ToList();

        var ventaIds = claves.Select(k => k.VentaId).Distinct().ToList();
        var tiendaIds = claves.Select(k => k.TiendaId).Distinct().ToList();

        // Consulta ventas existentes para evitar insertar duplicados
        var existentes = await _context.Ventas
            .Where(v => ventaIds.Contains(v.VentaId) && tiendaIds.Contains(v.TiendaId))
            .ToListAsync();

        // Busca claves que ya existen para evitar conflictos
        var conflictos = existentes.Where(e => claves.Any(k =>
            k.VentaId == e.VentaId &&
            k.TiendaId == e.TiendaId)).ToList();

        if (conflictos.Any())
        {
            // Retorna error 409 indicando que ya existen ventas con las claves proporcionadas
            return Conflict("Ya existen ventas con algunas de las claves proporcionadas.");
        }

        _context.Ventas.AddRange(ventas);
        await _context.SaveChangesAsync();

        // Retorna la cantidad de ventas insertadas para confirmar la operación
        return Ok(new { insertedCount = ventas.Count });
    }
}

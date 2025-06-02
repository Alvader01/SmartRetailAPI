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
    /// </summary>
    /// <returns>Lista de objetos Venta con sus detalles.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Venta>>> Get() =>
        await _context.Ventas.Include(v => v.DetallesVenta).ToListAsync();

    /// <summary>
    /// Obtiene una venta específica por su VentaId y TiendaId.
    /// </summary>
    /// <param name="id">Identificador de la venta (VentaId).</param>
    /// <param name="tiendaId">Identificador de la tienda (TiendaId).</param>
    /// <returns>Venta con sus detalles o NotFound si no existe.</returns>
    [HttpGet("{id}/{tiendaId}")]
    public async Task<ActionResult<Venta>> Get(int id, string tiendaId)
    {
        var venta = await _context.Ventas
            .Include(v => v.DetallesVenta) // Incluye detalles relacionados
            .FirstOrDefaultAsync(v => v.VentaId == id && v.TiendaId == tiendaId);

        if (venta == null)
        {
            return NotFound();
        }

        return Ok(venta); // Es mejor devolver Ok explícitamente
    }


    /// <summary>
    /// Crea una nueva venta en la base de datos.
    /// </summary>
    /// <param name="venta">Objeto Venta a insertar.</param>
    /// <returns>Venta creada con código HTTP 201 y ubicación.</returns>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] List<Venta> ventas)
    {
        if (ventas == null || ventas.Count == 0)
            return BadRequest("No se recibieron ventas.");

        if (ventas.Any(v => string.IsNullOrEmpty(v.TiendaId)))
            return BadRequest("Todas las ventas deben tener TiendaId.");

        _context.Ventas.AddRange(ventas);
        await _context.SaveChangesAsync();

        return Ok(new { insertedCount = ventas.Count });
    }

}

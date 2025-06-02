using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRetailApi.Data;
using SmartRetailApi.Models;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Constructor que inyecta el contexto de la base de datos.
    /// </summary>
    /// <param name="context">Contexto de base de datos para acceder a Productos.</param>
    public ProductosController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todos los productos almacenados en la base de datos.
    /// </summary>
    /// <returns>Lista de objetos Producto.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> Get()
    {
        return await _context.Productos.ToListAsync();
    }
    /// <summary>
    /// Obtiene un producto específico por su ProductoId y TiendaId.
    /// </summary>
    /// <param name="id">Identificador del producto (ProductoId).</param>
    /// <param name="tiendaId">Identificador de la tienda (TiendaId).</param>
    /// <returns>Producto encontrado o NotFound si no existe.</returns>
    [HttpGet("{id}/{tiendaId}")]
    public async Task<ActionResult<Producto>> Get(int id, string tiendaId)
    {
        var producto = await _context.Productos
            .FirstOrDefaultAsync(p => p.ProductoId == id && p.TiendaId == tiendaId);

        if (producto == null)
        {
            return NotFound();
        }

        return Ok(producto);
    }

    /// <summary>
    /// Crea un nuevo producto en la base de datos.
    /// </summary>
    /// <param name="producto">Objeto Producto enviado desde el cliente.</param>
    /// <returns>Producto creado y la ubicación para consultarlo.</returns>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] List<Producto> productos)
    {
        if (productos == null || productos.Count == 0)
            return BadRequest("No se recibieron productos.");

        if (productos.Any(p => string.IsNullOrEmpty(p.TiendaId)))
            return BadRequest("Todos los productos deben tener TiendaId.");

        _context.Productos.AddRange(productos);
        await _context.SaveChangesAsync();

        return Ok(new { insertedCount = productos.Count });
    }

}

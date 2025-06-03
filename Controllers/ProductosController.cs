using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRetailApi.Data;
using SmartRetailApi.Models;

[Authorize] // Requiere que el usuario esté autenticado para acceder a los endpoints
[ApiController] // Marca la clase como controlador API
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
    /// Considerar paginación o filtros si la tabla es muy grande.
    /// </summary>
    /// <returns>Lista de objetos Producto.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> Get()
    {
        var productos = await _context.Productos
            .Select(p => new
            {
                p.ProductoId,
                p.TiendaId,
                p.Nombre,
                p.Precio,
                p.Stock
            })
            .ToListAsync();

        return Ok(productos);
    }


    /// <summary>
    /// Obtiene un producto específico por su ProductoId y TiendaId.
    /// La combinación de estos dos campos identifica unívocamente cada producto.
    /// </summary>
    /// <param name="id">Identificador del producto (ProductoId).</param>
    /// <param name="tiendaId">Identificador de la tienda (TiendaId).</param>
    /// <returns>Producto encontrado o NotFound si no existe.</returns>
    [HttpGet("{id}/{tiendaId}")]
    public async Task<ActionResult<Producto>> Get(Guid id, string tiendaId)
    {
        var producto = await _context.Productos
            .FirstOrDefaultAsync(p => p.ProductoId == id && p.TiendaId == tiendaId);

        if (producto == null)
        {
            return NotFound();
        }

        return Ok(producto); // Retorna explícitamente código 200 OK con el producto
    }

    /// <summary>
    /// Inserta o actualiza (upsert) una lista de productos en la base de datos.
    /// Valida que la lista no sea nula o vacía y que todos los productos tengan TiendaId.
    /// Si un producto ya existe (misma clave compuesta ProductoId + TiendaId), se actualizan sus campos.
    /// </summary>
    /// <param name="productos">Lista de objetos Producto enviados desde el cliente.</param>
    /// <returns>Cantidad de productos procesados (insertados o actualizados) o error si la petición es inválida.</returns>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] List<Producto> productos)
    {
        if (productos == null || productos.Count == 0)
            return BadRequest("No se recibieron productos.");

        if (productos.Any(p => string.IsNullOrEmpty(p.TiendaId)))
            return BadRequest("Todos los productos deben tener TiendaId.");

        foreach (var producto in productos)
        {
            // Buscar producto existente por clave compuesta
            var productoExistente = await _context.Productos
                .FirstOrDefaultAsync(p => p.ProductoId == producto.ProductoId &&
                                          p.TiendaId == producto.TiendaId);

            if (productoExistente == null)
            {
                // Si no existe, agregar nuevo producto
                _context.Productos.Add(producto);
            }
            else
            {
                // Si existe, actualizar campos relevantes
                productoExistente.Nombre = producto.Nombre;
                productoExistente.Precio = producto.Precio;
                productoExistente.Stock = producto.Stock;
            }
        }

        await _context.SaveChangesAsync();

        // Retorna un resultado con la cantidad de registros procesados (insertados + actualizados)
        return Ok(new { processedCount = productos.Count });
    }
}

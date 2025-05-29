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
    /// Crea un nuevo producto en la base de datos.
    /// </summary>
    /// <param name="producto">Objeto Producto a insertar.</param>
    /// <returns>Producto creado con código HTTP 201 y ubicación.</returns>
    [HttpPost]
    public async Task<ActionResult<Producto>> Post(Producto producto)
    {
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = producto.ProductoId }, producto);
    }
}

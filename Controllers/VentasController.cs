using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRetailApi.Data;
using SmartRetailApi.Models;

[Authorize]
[ApiController]
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
    /// Crea una nueva venta en la base de datos.
    /// </summary>
    /// <param name="venta">Objeto Venta a insertar.</param>
    /// <returns>Venta creada con código HTTP 201 y ubicación.</returns>
    [HttpPost]
    public async Task<ActionResult<Venta>> Post(Venta venta)
    {
        _context.Ventas.Add(venta);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = venta.VentaId }, venta);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRetailApi.Data;
using SmartRetailApi.Models;

[Authorize]
[ApiController]
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
    /// Crea un nuevo detalle de venta en la base de datos.
    /// </summary>
    /// <param name="detalle">Objeto DetalleVenta a insertar.</param>
    /// <returns>DetalleVenta creado con código HTTP 201 y ubicación.</returns>
    [HttpPost]
    public async Task<ActionResult<DetalleVenta>> Post(DetalleVenta detalle)
    {
        _context.DetallesVenta.Add(detalle);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = detalle.VentaId }, detalle);
    }
}

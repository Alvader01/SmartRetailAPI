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
    /// Devuelve todos los datos de venta y en cada detalle incluye ProductoId y demás campos.
    /// </summary>
    /// <returns>Lista de objetos Venta con sus detalles.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> Get()
    {
        var ventas = await _context.Ventas
            .Include(v => v.Cliente) // Incluir Cliente
            .Select(v => new
            {
                v.VentaId,
                v.TiendaId,
                v.Fecha,
                v.Total,
                v.ClienteId,
                Cliente = new
                {
                    v.Cliente!.ClienteId,
                    v.Cliente.Nombre,
                    v.Cliente.Email
                    // Otros campos de Cliente que quieras exponer
                }
                // No incluimos DetallesVenta aquí
            })
            .ToListAsync();

        return Ok(ventas);
    }


    /// <summary>
    /// Obtiene una venta específica por su VentaId y TiendaId.
    /// La combinación de VentaId y TiendaId es la clave única para identificar la venta.
    /// </summary>
    /// <param name="id">Identificador de la venta (VentaId).</param>
    /// <param name="tiendaId">Identificador de la tienda (TiendaId).</param>
    /// <returns>Venta con sus detalles o NotFound si no existe.</returns>
    [HttpGet("{id}/{tiendaId}")]
    public async Task<ActionResult<object>> Get(Guid id, string tiendaId)
    {
        var venta = await _context.Ventas
            .Include(v => v.Cliente) // Incluir Cliente
            .Include(v => v.DetallesVenta!)
                .ThenInclude(dv => dv.Producto) // Incluir Producto en cada detalle
            .Where(v => v.VentaId == id && v.TiendaId == tiendaId)
            .Select(v => new
            {
                v.VentaId,
                v.TiendaId,
                v.Fecha,
                v.Total,
                v.ClienteId,
                Cliente = new
                {
                    v.Cliente!.ClienteId,
                    v.Cliente.Nombre,
                    v.Cliente.Email
                },
                DetallesVenta = v.DetallesVenta!.Select(dv => new
                {
                    dv.VentaId,
                    dv.ProductoId,
                    dv.TiendaId,
                    dv.Cantidad,
                    dv.Subtotal,
                    Producto = new
                    {
                        dv.Producto!.ProductoId,
                        dv.Producto.Nombre,
                        dv.Producto.Precio
                    }
                })
            })
            .FirstOrDefaultAsync();

        if (venta == null)
            return NotFound();

        return Ok(venta);
    }

    /// <summary>
    /// Crea o actualiza una o varias ventas en la base de datos.
    /// Valida que la lista no sea nula o vacía y que todas las ventas tengan TiendaId.
    /// Realiza un upsert: inserta nuevas ventas y actualiza las existentes (incluyendo sus detalles).
    /// </summary>
    /// <param name="ventas">Lista de objetos Venta a insertar o actualizar.</param>
    /// <returns>Cantidad de ventas insertadas o actualizadas, o error si la petición es inválida.</returns>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] List<Venta> ventas)
    {
        if (ventas == null || ventas.Count == 0)
            return BadRequest("No se recibieron ventas.");

        if (ventas.Any(v => string.IsNullOrEmpty(v.TiendaId)))
            return BadRequest("Todas las ventas deben tener TiendaId.");

        var claves = ventas.Select(v => new { v.VentaId, v.TiendaId }).ToList();
        var ventaIds = claves.Select(k => k.VentaId).Distinct().ToList();
        var tiendaIds = claves.Select(k => k.TiendaId).Distinct().ToList();

        var existentes = await _context.Ventas
            .Include(v => v.DetallesVenta)
            .Where(v => ventaIds.Contains(v.VentaId) && tiendaIds.Contains(v.TiendaId))
            .ToListAsync();

        foreach (var venta in ventas)
        {
            // 🔧 Corrección clave: eliminar referencias a entidades navegacionales
            venta.Cliente = null;
            if (venta.DetallesVenta != null)
            {
                foreach (var detalle in venta.DetallesVenta)
                {
                    detalle.Producto = null;
                }
            }

            var existente = existentes
                .FirstOrDefault(e => e.VentaId == venta.VentaId && e.TiendaId == venta.TiendaId);

            if (existente == null)
            {
                _context.Ventas.Add(venta);
            }
            else
            {
                existente.Fecha = venta.Fecha;
                existente.Total = venta.Total;
                existente.ClienteId = venta.ClienteId;

                if (existente.DetallesVenta != null)
                    _context.DetallesVenta.RemoveRange(existente.DetallesVenta);

                existente.DetallesVenta = venta.DetallesVenta;
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new { upsertedCount = ventas.Count });
    }
}

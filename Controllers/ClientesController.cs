using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRetailApi.Data;
using SmartRetailApi.Models;

[Authorize] // Requiere autenticación para todas las acciones del controlador
[ApiController] // Marca la clase como controlador API con funcionalidades automáticas
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Constructor que inicializa el contexto de base de datos para Clientes.
    /// </summary>
    /// <param name="context">Instancia de AppDbContext para acceso a datos.</param>
    public ClientesController(AppDbContext context) => _context = context;

    /// <summary>
    /// Obtiene la lista completa de clientes en todas las tiendas.
    /// Considera que esto puede devolver muchos datos si la base es grande.
    /// </summary>
    /// <returns>Lista de clientes en formato IEnumerable.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> Get()
    {
        // Considera devolver solo campos necesarios y omitir la colección Ventas para no cargar datos innecesarios
        var clientes = await _context.Clientes
            .Select(c => new
            {
                c.ClienteId,
                c.TiendaId,
                c.Nombre,
                c.Correo,
                c.Telefono
            })
            .ToListAsync();

        return Ok(clientes);
    }

    /// <summary>
    /// Obtiene un cliente específico por su clave compuesta ClienteId y TiendaId.
    /// Esta combinación debe ser única para identificar el cliente.
    /// </summary>
    /// <param name="clienteId">Identificador del cliente.</param>
    /// <param name="tiendaId">Identificador de la tienda.</param>
    /// <returns>Cliente encontrado o NotFound si no existe.</returns>
    [HttpGet("{clienteId}/{tiendaId}")]
    public async Task<ActionResult<object>> Get(Guid clienteId, string tiendaId)
    {
        var cliente = await _context.Clientes
            .Where(c => c.ClienteId == clienteId && c.TiendaId == tiendaId)
            .Select(c => new
            {
                c.ClienteId,
                c.TiendaId,
                c.Nombre,
                c.Correo,
                c.Telefono
            })
            .FirstOrDefaultAsync();

        if (cliente == null)
            return NotFound();

        return Ok(cliente);
    }

    /// <summary>
    /// Inserta o actualiza (upsert) una lista de clientes en la base de datos.
    /// Se valida que la lista no esté vacía y que cada cliente tenga TiendaId definido.
    /// Si un cliente ya existe (misma clave compuesta ClienteId+TiendaId), se actualizan sus campos.
    /// </summary>
    /// <param name="clientes">Lista de objetos Cliente con los datos a insertar o actualizar.</param>
    /// <returns>Resultado con número de clientes insertados/actualizados o error si la petición es inválida.</returns>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] List<Cliente> clientes)
    {
        if (clientes == null || clientes.Count == 0)
            return BadRequest("No se recibieron clientes.");

        if (clientes.Any(c => string.IsNullOrEmpty(c.TiendaId)))
            return BadRequest("Todos los clientes deben tener TiendaId.");

        foreach (var cliente in clientes)
        {
            var clienteExistente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.ClienteId == cliente.ClienteId && c.TiendaId == cliente.TiendaId);

            if (clienteExistente == null)
            {
                _context.Clientes.Add(cliente);
            }
            else
            {
                clienteExistente.Nombre = cliente.Nombre;
                clienteExistente.Correo = cliente.Correo;
                clienteExistente.Telefono = cliente.Telefono;
                
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new { processedCount = clientes.Count });
    }
}
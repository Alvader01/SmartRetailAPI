using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRetailApi.Data;
using SmartRetailApi.Models;

[Authorize] // Requiere autenticación para todas las acciones
[ApiController] // Controlador API
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
    /// Obtiene la lista completa de clientes.
    /// </summary>
    /// <returns>Lista de clientes en formato IEnumerable.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> Get() =>
        await _context.Clientes.ToListAsync();

    /// <summary>
    /// Obtiene un cliente específico por su clave compuesta ClienteId y TiendaId.
    /// </summary>
    /// <param name="clienteId">Identificador del cliente.</param>
    /// <param name="tiendaId">Identificador de la tienda.</param>
    /// <returns>Cliente encontrado o NotFound si no existe.</returns>
    [HttpGet("{clienteId}/{tiendaId}")]
    public async Task<ActionResult<Cliente>> Get(int clienteId, string tiendaId)
    {
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.ClienteId == clienteId && c.TiendaId == tiendaId);

        if (cliente == null)
        {
            return NotFound();
        }

        return Ok(cliente); // Devolver Ok explícitamente es buena práctica
    }


    /// <summary>
    /// Crea un nuevo cliente en la base de datos.
    /// </summary>
    /// <param name="cliente">Objeto Cliente con los datos a insertar.</param>
    /// <returns>Cliente creado con código HTTP 201 y ubicación del recurso.</returns>
    [HttpPost]
    public async Task<ActionResult<Cliente>> Post(Cliente cliente)
    {
        if (string.IsNullOrEmpty(cliente.TiendaId))
        {
            return BadRequest("TiendaId es obligatorio.");
        }

        // No asignar ClienteId manualmente, la DB lo genera (autoincremental)
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        // Devuelve 201 Created con la ruta completa a este recurso
        return CreatedAtAction(nameof(Get), new { clienteId = cliente.ClienteId, tiendaId = cliente.TiendaId }, cliente);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRetailApi.Data;
using SmartRetailApi.Models;

[Authorize]
[ApiController]
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
    /// Crea un nuevo cliente en la base de datos.
    /// </summary>
    /// <param name="cliente">Objeto Cliente con los datos a insertar.</param>
    /// <returns>Cliente creado con código HTTP 201 y ubicación del recurso.</returns>
    [HttpPost]
    public async Task<ActionResult<Cliente>> Post(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = cliente.ClienteId }, cliente);
    }
}

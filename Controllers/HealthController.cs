using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRetailApi.Data;

namespace SmartRetailApi.Controllers
{
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Constructor que inyecta el contexto de base de datos.
        /// </summary>
        /// <param name="context">Contexto de base de datos para verificar la conexión.</param>
        public HealthController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Endpoint de comprobación de salud de la API y conexión a base de datos.
        /// </summary>
        /// <remarks>
        /// Este endpoint puede ser usado por servicios como Render para verificar el estado de la API.
        /// Comprueba tanto que la API responde como que la base de datos está accesible.
        /// </remarks>
        /// <returns>
        /// Código 200 OK si todo está correcto, o 500 si falla la conexión a la base de datos.
        /// </returns>
        [HttpGet("/healthz")]
        [AllowAnonymous]
        public async Task<IActionResult> HealthCheck()
        {
            try
            {
                // Intenta hacer una consulta mínima a la base de datos.
                var canConnect = await _context.Database.CanConnectAsync();
                if (!canConnect)
                    return StatusCode(500, "Error: No se pudo conectar con la base de datos.");

                return Ok("Healthy");
            }
            catch (Exception ex)
            {
                // Devuelve un error 500 en caso de cualquier excepción.
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}

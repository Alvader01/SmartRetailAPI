using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartRetailApi.Controllers
{
    /// <summary>
    /// Controlador para la autenticación de usuarios y generación de tokens JWT.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor que inyecta la configuración de la aplicación.
        /// </summary>
        /// <param name="configuration">Objeto de configuración para acceder a appsettings.json.</param>
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Endpoint para login de usuario.
        /// Valida las credenciales recibidas contra las credenciales almacenadas en la cadena de conexión.
        /// Este método puede adaptarse para validar contra otros orígenes, como base de datos o servicios externos.
        /// </summary>
        /// <param name="request">Objeto con usuario y contraseña enviados desde el cliente.</param>
        /// <returns>JWT en caso de éxito o respuesta 401 Unauthorized si falla.</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Obtiene la cadena de conexión configurada en appsettings.json
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            // Construye un objeto para acceder a los datos de conexión fácilmente
            var builder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);

            // Compara las credenciales recibidas con las almacenadas en la cadena de conexión
            // NOTA: Para producción, se recomienda validar contra una tabla de usuarios cifrados
            if (request.Username == builder.Username && request.Password == builder.Password)
            {
                // Genera un token JWT para el usuario autenticado
                var token = GenerateJwtToken(request.Username);
                return Ok(new { Token = token });
            }
            else
            {
                // Si las credenciales no coinciden, devuelve un error 401 Unauthorized
                return Unauthorized("Usuario o contraseña incorrectos");
            }
        }

        /// <summary>
        /// Método privado que genera un token JWT para el usuario dado.
        /// El token incluye el nombre de usuario como claim y tiene un tiempo de expiración configurable.
        /// </summary>
        /// <param name="username">Nombre de usuario para incluir en el token.</param>
        /// <returns>Token JWT en formato string.</returns>
        private string GenerateJwtToken(string username)
        {
            // Obtiene la configuración JWT de appsettings.json (Key, Issuer, Audience, Duración)
            var jwtSettings = _configuration.GetSection("Jwt");

            // Clave secreta para firmar el token (debe mantenerse segura y fuera del código fuente)
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            // Crea el manejador para tokens JWT
            var tokenHandler = new JwtSecurityTokenHandler();

            // Define las propiedades y reclamaciones del token JWT
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                    // Se pueden añadir más claims aquí, como roles, permisos, tienda asociada, etc.
                }),
                // Tiempo de expiración del token (en minutos) configurado en appsettings.json
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["DurationInMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Crea el token JWT con el descriptor configurado
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Devuelve el token en formato string para ser usado por el cliente
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Clase que representa el cuerpo de la petición de login.
        /// Contiene las propiedades Username y Password.
        /// </summary>
        public class LoginRequest
        {
            /// <summary>
            /// Nombre de usuario enviado por el cliente.
            /// </summary>
            public string Username { get; set; } = null!;

            /// <summary>
            /// Contraseña del usuario enviada por el cliente.
            /// </summary>
            public string Password { get; set; } = null!;
        }
    }
}

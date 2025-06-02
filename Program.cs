using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartRetailApi.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuración del contexto de base de datos para usar PostgreSQL con la cadena de conexión definida en appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agrega soporte para controladores API
builder.Services.AddControllers();

// Obtiene la configuración JWT desde appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key no configurada"));

// Configuración del middleware de autenticación JWT
builder.Services.AddAuthentication(options =>
{
    // Define el esquema de autenticación por defecto como JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    // En desarrollo se permite HTTP para facilitar pruebas, en producción debe ser true para mayor seguridad

    options.SaveToken = true; // Guarda el token en el contexto para posteriores accesos

    // Parámetros para validar el token JWT
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Valida el emisor
        ValidateAudience = true, // Valida la audiencia
        ValidateLifetime = true, // Valida que el token no esté expirado
        ValidateIssuerSigningKey = true, // Valida la firma del token

        ValidIssuer = jwtSettings["Issuer"], // Emisor válido definido en configuración
        ValidAudience = jwtSettings["Audience"], // Audiencia válida definida en configuración
        IssuerSigningKey = new SymmetricSecurityKey(key), // Clave para validar la firma del token

        ClockSkew = TimeSpan.Zero // Elimina tolerancia por defecto en expiración para mayor precisión
    };
});

// Configuración de Swagger para la documentación y testing de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SmartRetail API",
        Version = "v1",
        Description = "API para gestión de SmartRetail con autenticación JWT"
    });

    // Configura esquema de seguridad para permitir autenticación JWT en Swagger UI
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Introduce el token JWT como: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

var app = builder.Build();

// Configuración para entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Página de errores detallada para desarrollo
    app.UseSwagger();                // Habilita Swagger
    app.UseSwaggerUI(c =>           // Interfaz de Swagger para pruebas
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartRetail API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz (opcional)
    });
}

app.UseHttpsRedirection(); // Fuerza redirección a HTTPS para seguridad

app.UseAuthentication(); // Middleware para autenticación JWT
app.UseAuthorization();  // Middleware para autorización basada en roles o políticas

app.MapControllers(); // Mapea rutas a controladores

app.Run(); // Ejecuta la aplicación

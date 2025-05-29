using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartRetailApi.Data;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuración del contexto de base de datos para usar PostgreSQL con la cadena de conexión definida en appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agrega soporte para controladores API
builder.Services.AddControllers();

// Obtiene la configuración JWT desde appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

// Configuración del middleware de autenticación JWT
builder.Services.AddAuthentication(options =>
{
    // Define el esquema de autenticación por defecto como JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // En desarrollo no se requiere HTTPS, en producción debe ser true
    options.SaveToken = true; // Guarda el token en el contexto para posteriores accesos

    // Parámetros para validar el token JWT
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Valida el emisor
        ValidateAudience = true, // Valida la audiencia
        ValidIssuer = jwtSettings["Issuer"], // Emisor válido definido en configuración
        ValidAudience = jwtSettings["Audience"], // Audiencia válida definida en configuración
        IssuerSigningKey = new SymmetricSecurityKey(key) // Clave para validar la firma del token
    };
});

// Configuración de Swagger para la documentación y testing de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartRetail API", Version = "v1" });

    // Configura esquema de seguridad para permitir autenticación JWT en Swagger UI
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Introduce el token JWT como: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new[] { "Bearer" } }
    });
});

var app = builder.Build();

// Configuración para entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Página de errores detallada
    app.UseSwagger(); // Habilita Swagger
    app.UseSwaggerUI(); // Interfaz de Swagger para pruebas
}

app.UseHttpsRedirection(); // Fuerza redirección a HTTPS

app.UseAuthentication(); // Middleware para autenticación
app.UseAuthorization();  // Middleware para autorización

app.MapControllers(); // Mapea rutas a controladores

app.Run(); // Ejecuta la aplicación

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartRetailApi.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n del contexto de base de datos para usar PostgreSQL con la cadena de conexi�n definida en appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agrega soporte para controladores API
builder.Services.AddControllers();

// Obtiene la configuraci�n JWT desde appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key no configurada"));

// Configuraci�n del middleware de autenticaci�n JWT
builder.Services.AddAuthentication(options =>
{
    // Define el esquema de autenticaci�n por defecto como JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    // En desarrollo se permite HTTP para facilitar pruebas, en producci�n debe ser true para mayor seguridad

    options.SaveToken = true; // Guarda el token en el contexto para posteriores accesos

    // Par�metros para validar el token JWT
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Valida el emisor
        ValidateAudience = true, // Valida la audiencia
        ValidateLifetime = true, // Valida que el token no est� expirado
        ValidateIssuerSigningKey = true, // Valida la firma del token

        ValidIssuer = jwtSettings["Issuer"], // Emisor v�lido definido en configuraci�n
        ValidAudience = jwtSettings["Audience"], // Audiencia v�lida definida en configuraci�n
        IssuerSigningKey = new SymmetricSecurityKey(key), // Clave para validar la firma del token

        ClockSkew = TimeSpan.Zero // Elimina tolerancia por defecto en expiraci�n para mayor precisi�n
    };
});

// Configuraci�n de Swagger para la documentaci�n y testing de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SmartRetail API",
        Version = "v1",
        Description = "API para gesti�n de SmartRetail con autenticaci�n JWT"
    });

    // Configura esquema de seguridad para permitir autenticaci�n JWT en Swagger UI
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

// Configuraci�n para entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // P�gina de errores detallada para desarrollo
    app.UseSwagger();                // Habilita Swagger
    app.UseSwaggerUI(c =>           // Interfaz de Swagger para pruebas
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartRetail API v1");
        c.RoutePrefix = string.Empty; // Swagger en la ra�z (opcional)
    });
}

app.UseHttpsRedirection(); // Fuerza redirecci�n a HTTPS para seguridad

app.UseAuthentication(); // Middleware para autenticaci�n JWT
app.UseAuthorization();  // Middleware para autorizaci�n basada en roles o pol�ticas

app.MapControllers(); // Mapea rutas a controladores

app.Run(); // Ejecuta la aplicaci�n

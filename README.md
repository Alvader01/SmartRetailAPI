SmartRetail API
Descripción
API REST segura desarrollada en ASP.NET Core para la gestión integral de productos, clientes, ventas y detalles de venta en la plataforma SmartRetail.
Implementa autenticación basada en tokens JWT y se conecta a una base de datos PostgreSQL alojada en la nube para mantener sincronizados los datos en tiempo real.

Esta API sirve como backend para aplicaciones clientes que necesiten interactuar con la base de datos de SmartRetail, ofreciendo endpoints para crear, consultar y gestionar información esencial para la operación comercial.

Características principales
Gestión de clientes, productos, ventas y detalles de venta.

Autenticación JWT para proteger los endpoints y asegurar el acceso.

Conexión segura a base de datos PostgreSQL en la nube.

Documentación automática con Swagger UI.

Requisitos
.NET 8 SDK

PostgreSQL (Base de datos en la nube configurada)

Editor de código (Visual Studio, VS Code, etc.)

Configuración
Clonar este repositorio.

Configurar el archivo appsettings.json con la cadena de conexión a tu base de datos PostgreSQL.

Configurar los parámetros JWT en appsettings.json para la generación y validación de tokens.

Restaurar paquetes con dotnet restore.

Ejecutar migraciones y levantar la API con dotnet run.

Uso
Autenticar el usuario enviando un POST a /api/auth/login con las credenciales de la base de datos.

Obtener el token JWT y usarlo en el header Authorization: Bearer {token} para acceder a los demás endpoints protegidos.

Consultar y gestionar recursos como clientes, productos, ventas y detalles mediante los endpoints RESTful disponibles.

Endpoints disponibles
Método	Ruta	Descripción
POST	/api/auth/login	Autenticación y generación de token JWT
GET	/api/clientes	Listar clientes
POST	/api/clientes	Añadir nuevo cliente
GET	/api/productos	Listar productos
POST	/api/productos	Añadir nuevo producto
GET	/api/ventas	Listar ventas
POST	/api/ventas	Añadir nueva venta
GET	/api/detallesventa	Listar detalles de venta
POST	/api/detallesventa	Añadir detalle de venta

Swagger UI
La API cuenta con documentación y prueba de endpoints vía Swagger UI disponible en entorno de desarrollo en:
https://{tu-url-api}/swagger

Licencia
Este proyecto está bajo la licencia MIT.


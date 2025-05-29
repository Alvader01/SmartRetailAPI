SmartRetail API
Descripci�n
API REST segura desarrollada en ASP.NET Core para la gesti�n integral de productos, clientes, ventas y detalles de venta en la plataforma SmartRetail.
Implementa autenticaci�n basada en tokens JWT y se conecta a una base de datos PostgreSQL alojada en la nube para mantener sincronizados los datos en tiempo real.

Esta API sirve como backend para aplicaciones clientes que necesiten interactuar con la base de datos de SmartRetail, ofreciendo endpoints para crear, consultar y gestionar informaci�n esencial para la operaci�n comercial.

Caracter�sticas principales
Gesti�n de clientes, productos, ventas y detalles de venta.

Autenticaci�n JWT para proteger los endpoints y asegurar el acceso.

Conexi�n segura a base de datos PostgreSQL en la nube.

Documentaci�n autom�tica con Swagger UI.

Requisitos
.NET 8 SDK

PostgreSQL (Base de datos en la nube configurada)

Editor de c�digo (Visual Studio, VS Code, etc.)

Configuraci�n
Clonar este repositorio.

Configurar el archivo appsettings.json con la cadena de conexi�n a tu base de datos PostgreSQL.

Configurar los par�metros JWT en appsettings.json para la generaci�n y validaci�n de tokens.

Restaurar paquetes con dotnet restore.

Ejecutar migraciones y levantar la API con dotnet run.

Uso
Autenticar el usuario enviando un POST a /api/auth/login con las credenciales de la base de datos.

Obtener el token JWT y usarlo en el header Authorization: Bearer {token} para acceder a los dem�s endpoints protegidos.

Consultar y gestionar recursos como clientes, productos, ventas y detalles mediante los endpoints RESTful disponibles.

Endpoints disponibles
M�todo	Ruta	Descripci�n
POST	/api/auth/login	Autenticaci�n y generaci�n de token JWT
GET	/api/clientes	Listar clientes
POST	/api/clientes	A�adir nuevo cliente
GET	/api/productos	Listar productos
POST	/api/productos	A�adir nuevo producto
GET	/api/ventas	Listar ventas
POST	/api/ventas	A�adir nueva venta
GET	/api/detallesventa	Listar detalles de venta
POST	/api/detallesventa	A�adir detalle de venta

Swagger UI
La API cuenta con documentaci�n y prueba de endpoints v�a Swagger UI disponible en entorno de desarrollo en:
https://{tu-url-api}/swagger

Licencia
Este proyecto est� bajo la licencia MIT.


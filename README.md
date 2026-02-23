# wtw

Microservicio en esta desarrollado en ASP.NET Core (.NET 8) para gestión de facturas en SQL Server con Stored Procedures.

## Alcance de la prueba

Este repositorio cubre:

- **Parte 1**: Diseño, implementación y documentación de una **API REST** para registrar y buscar facturas en **SQL Server**.
- **Parte 2**: Optimización de prompt para resumen en `PROMPTS.md`.
- **Parte 3**: La estrategia de **pruebas volumétricas** en `VOLUMETRIC_TESTING.md`.

## Requisitos

- .NET SDK **8.x**
- Docker (para SQL Server)

## Para desplegar SQL Server se uso Docker

En la raíz del proyecto:

```bash
docker compose up -d
```

Esto crea/despliegua:

- `sqlserver` (SQL Server 2022)
- `sql-init` (contenedor que ejecuta scripts de inicialización)

### Base de datos y scripts

Los scripts se ejecutan automáticamente en Docker:

- `db/init/001_schema.sql`
  - Crea la base `InvoiceDb`
  - Crea la tabla `dbo.Facturas`
  - Crea índice `IX_Facturas_ClienteNombre`
- `db/init/002_stored_procedures.sql`
  - `dbo.SP_InsertarFactura`
  - `dbo.SP_ObtenerFacturaPorId`
  - `dbo.SP_BuscarFacturasPorCliente`

## Ejecutar la API

```bash
dotnet run --project InvoiceService.Api
```

### URL local

La API expone por defecto en el puerto 5109 y 7192 para despliegues locales (ver `InvoiceService.Api/Properties/launchSettings.json`):

- `http://localhost:5109`
- `https://localhost:7192`

Swagger:

- `http://localhost:5109/swagger`
- `https://localhost:7192/swagger`

## Autenticación y roles

La API usa **JWT Bearer** con roles:

- `Administrador`: puede **crear y consultar**
- `Usuario`: solo puede **consultar**

### Login

- `POST /auth/login`

Regla de demo:

- Si `username = "admin"` se emite token con rol `Administrador`
- Cualquier otro `username` se emite token con rol `Usuario`

En Swagger:

1. Ejecuta `POST /auth/login` para obtener el token
2. En Swagger, presiona **Authorize** y pega `Bearer <TOKEN>`

## Endpoints (Parte 1)

### 1) Crear factura

- `POST /invoice`
- Requiere rol: `Administrador`

Validaciones (FluentValidation):

- Campos requeridos no vacíos
- `ClienteNombre` no debe contener números
- `Total > 0`
- `FechaVencimiento > FechaEmision`
- `Estado` permitido: `Pendiente | Pagada | Anulada`

Persistencia:

- **Sin Entity Framework**
- Inserción vía Stored Procedure: `dbo.SP_InsertarFactura`

### 2) Obtener factura por id

- `GET /invoice/{id}`
- Roles: `Administrador`, `Usuario`

Consulta:

- Stored Procedure: `dbo.SP_ObtenerFacturaPorId`

Si no existe:

- `404 Not Found` con mensaje descriptivo

### 3) Buscar facturas por cliente

- `GET /invoice/search?client={clientName}`
- Roles: `Administrador`, `Usuario`

Consulta:

- Stored Procedure: `dbo.SP_BuscarFacturasPorCliente`

Optimización:

- Índice en `ClienteNombre`: `IX_Facturas_ClienteNombre`

Si no hay resultados:

- `200 OK` con lista vacía `[]`

## Formato estándar de respuesta

Todas las respuestas siguen:

```json
{
  "success": true,
  "data": {},
  "message": "string",
  "errors": []
}
```

## Manejo de errores

Implementado con middleware global y manejo de validaciones/autenticación:

- `400 Bad Request`: datos inválidos (ModelState/FluentValidation)
- `401 Unauthorized`: token ausente o inválido (JWT events)
- `403 Forbidden`: rol sin permisos (JWT events)
- `404 Not Found`: factura no encontrada
- `409 Conflict`: `NumeroFactura` duplicado (UNIQUE + mapeo de error)
- `500 Internal Server Error`: error inesperado

## Swagger con ejemplos

Swagger incluye ejemplos de request/response por endpoint mediante `SwaggerExamplesOperationFilter`.

## Pruebas unitarias

Proyecto: `InvoiceService.Tests`

Frameworks:

- xUnit
- Moq
- FluentAssertions

Cobertura:

- Tests de **Service** (`InvoiceService`) usando mocks del repositorio
- Tests de **Validator** (`InvoiceCreateDtoValidator`)

Ejecutar:

```bash
dotnet test
```

## Parte 2: optimización de prompt

Ver `PROMPTS.md`.

## Parte 3: pruebas volumétricas

Ver `VOLUMETRIC_TESTING.md`.

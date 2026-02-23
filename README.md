# wtw

Microservicio en ASP.NET Core (.NET 8) para gestión de facturas en SQL Server con Stored Procedures.

## Alcance de la prueba

Este repositorio cubre:

- **Parte 1**: diseño, implementación y documentación de una **API REST** para registrar y buscar facturas en **SQL Server**.
- **Parte 2**: optimización de prompt para resumen en `PROMPTS.md`.
- **Parte 3**: estrategia de **pruebas volumétricas** en `VOLUMETRIC_TESTING.md`.

## Requisitos

- .NET SDK **8.x**
- Docker (para SQL Server)

## Levantar SQL Server (Docker)

En la raíz del proyecto:

```bash
docker compose up -d
```

Esto crea/levanta:

- `sqlserver` (SQL Server 2022)
- `sql-init` (contenedor que ejecuta scripts de inicialización)

### Base de datos y scripts

Los scripts se ejecutan automáticamente al levantar Docker:

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

La API expone por defecto (ver `InvoiceService.Api/Properties/launchSettings.json`):

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

### cURL (importable en Postman) - Login

Obtener token de **Administrador** (usuario `admin`):

```bash
curl -X POST "http://localhost:5109/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "admin123"
  }'
```

Obtener token de **Usuario** (cualquier username diferente a `admin`):

```bash
curl -X POST "http://localhost:5109/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "user1",
    "password": "user123"
  }'
```

Opcional: exportar el token a una variable `TOKEN` (pega manualmente el token devuelto):

```bash
export TOKEN="<PEGAR_TOKEN_AQUI>"
```

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

Nota: `POST /invoice` es **solo creación**. Si intentas enviar el mismo `numeroFactura` de nuevo para “cambiar estado”, fallará con `409` por restricción `UNIQUE`.

#### cURL (importable en Postman) - POST /invoice

Requiere token con rol `Administrador`.

```bash
curl -X POST "http://localhost:5109/invoice" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "numeroFactura": "F-2026-0001",
    "clienteNombre": "Juan Perez",
    "clienteEmail": "juan.perez@example.com",
    "fechaEmision": "2026-02-23T10:00:00",
    "fechaVencimiento": "2026-03-05T10:00:00",
    "subtotal": 100.00,
    "impuesto": 19.00,
    "total": 119.00,
    "estado": "Pendiente"
  }'
```

### 2) Obtener factura por id

- `GET /invoice/{id}`
- Roles: `Administrador`, `Usuario`

Consulta:

- Stored Procedure: `dbo.SP_ObtenerFacturaPorId`

Si no existe:

- `404 Not Found` con mensaje descriptivo

#### cURL (importable en Postman) - GET /invoice/{id}

```bash
curl -X GET "http://localhost:5109/invoice/1" \
  -H "Authorization: Bearer $TOKEN"
```

### 2.1) Actualizar estado de una factura

- `PATCH /invoice/{id}/status`
- Requiere rol: `Administrador`

Persistencia:

- Stored Procedure: `dbo.SP_ActualizarEstadoFactura`

#### cURL (importable en Postman) - PATCH /invoice/{id}/status

```bash
curl -X PATCH "http://localhost:5109/invoice/1/status" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "estado": "Pagada"
  }'
```

### 3) Buscar facturas por cliente

- `GET /invoice/search?client={clientName}`
- Roles: `Administrador`, `Usuario`

Consulta:

- Stored Procedure: `dbo.SP_BuscarFacturasPorCliente`

Optimización:

- Índice en `ClienteNombre`: `IX_Facturas_ClienteNombre`

Si no hay resultados:

- `200 OK` con lista vacía `[]`

#### cURL (importable en Postman) - GET /invoice/search

```bash
curl -X GET "http://localhost:5109/invoice/search?client=Juan" \
  -H "Authorization: Bearer $TOKEN"
```

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

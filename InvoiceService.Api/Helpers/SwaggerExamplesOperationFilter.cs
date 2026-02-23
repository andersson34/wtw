using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace InvoiceService.Api.Helpers;

public sealed class SwaggerExamplesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var path = "/" + (context.ApiDescription.RelativePath ?? string.Empty).TrimStart('/');
        var method = context.ApiDescription.HttpMethod ?? string.Empty;

        if (method.Equals("POST", StringComparison.OrdinalIgnoreCase) && path.Equals("/auth/login", StringComparison.OrdinalIgnoreCase))
        {
            SetRequestExample(operation, new OpenApiObject
            {
                ["username"] = new OpenApiString("admin"),
                ["password"] = new OpenApiString("admin123")
            });

            SetResponseExample(operation, "200", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(true),
                ["data"] = new OpenApiObject
                {
                    ["token"] = new OpenApiString("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...")
                },
                ["message"] = new OpenApiString("Token generado."),
                ["errors"] = new OpenApiArray()
            });

            SetResponseExample(operation, "400", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(false),
                ["data"] = new OpenApiNull(),
                ["message"] = new OpenApiString("Credenciales inválidas."),
                ["errors"] = new OpenApiArray()
            });

            return;
        }

        if (method.Equals("POST", StringComparison.OrdinalIgnoreCase) && path.Equals("/invoice", StringComparison.OrdinalIgnoreCase))
        {
            SetRequestExample(operation, new OpenApiObject
            {
                ["numeroFactura"] = new OpenApiString("F-2026-0001"),
                ["clienteNombre"] = new OpenApiString("Juan Perez"),
                ["clienteEmail"] = new OpenApiString("juan.perez@example.com"),
                ["fechaEmision"] = new OpenApiString("2026-02-23T10:00:00"),
                ["fechaVencimiento"] = new OpenApiString("2026-03-05T10:00:00"),
                ["subtotal"] = new OpenApiDouble(100.00),
                ["impuesto"] = new OpenApiDouble(19.00),
                ["total"] = new OpenApiDouble(119.00),
                ["estado"] = new OpenApiString("Pendiente")
            });

            SetResponseExample(operation, "201", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(true),
                ["data"] = new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(1),
                    ["numeroFactura"] = new OpenApiString("F-2026-0001"),
                    ["clienteNombre"] = new OpenApiString("Juan Perez"),
                    ["clienteEmail"] = new OpenApiString("juan.perez@example.com"),
                    ["fechaEmision"] = new OpenApiString("2026-02-23T10:00:00"),
                    ["fechaVencimiento"] = new OpenApiString("2026-03-05T10:00:00"),
                    ["subtotal"] = new OpenApiDouble(100.00),
                    ["impuesto"] = new OpenApiDouble(19.00),
                    ["total"] = new OpenApiDouble(119.00),
                    ["estado"] = new OpenApiString("Pendiente"),
                    ["creadoEn"] = new OpenApiString("2026-02-23T10:01:00Z")
                },
                ["message"] = new OpenApiString("Factura creada."),
                ["errors"] = new OpenApiArray()
            });

            SetResponseExample(operation, "400", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(false),
                ["data"] = new OpenApiNull(),
                ["message"] = new OpenApiString("Datos inválidos."),
                ["errors"] = new OpenApiArray
                {
                    new OpenApiString("ClienteNombre: debe ser solo texto"),
                    new OpenApiString("Total: debe ser mayor a 0")
                }
            });

            SetResponseExample(operation, "401", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(false),
                ["data"] = new OpenApiNull(),
                ["message"] = new OpenApiString("No autorizado."),
                ["errors"] = new OpenApiArray()
            });

            SetResponseExample(operation, "403", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(false),
                ["data"] = new OpenApiNull(),
                ["message"] = new OpenApiString("Prohibido."),
                ["errors"] = new OpenApiArray()
            });

            SetResponseExample(operation, "409", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(false),
                ["data"] = new OpenApiNull(),
                ["message"] = new OpenApiString("Número de factura duplicado."),
                ["errors"] = new OpenApiArray()
            });

            return;
        }

        if (method.Equals("GET", StringComparison.OrdinalIgnoreCase) && path.StartsWith("/invoice/{id}", StringComparison.OrdinalIgnoreCase))
        {
            SetResponseExample(operation, "200", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(true),
                ["data"] = new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(1),
                    ["numeroFactura"] = new OpenApiString("F-2026-0001"),
                    ["clienteNombre"] = new OpenApiString("Juan Perez"),
                    ["clienteEmail"] = new OpenApiString("juan.perez@example.com"),
                    ["fechaEmision"] = new OpenApiString("2026-02-23T10:00:00"),
                    ["fechaVencimiento"] = new OpenApiString("2026-03-05T10:00:00"),
                    ["subtotal"] = new OpenApiDouble(100.00),
                    ["impuesto"] = new OpenApiDouble(19.00),
                    ["total"] = new OpenApiDouble(119.00),
                    ["estado"] = new OpenApiString("Pendiente"),
                    ["creadoEn"] = new OpenApiString("2026-02-23T10:01:00Z")
                },
                ["message"] = new OpenApiString("Factura obtenida."),
                ["errors"] = new OpenApiArray()
            });

            SetResponseExample(operation, "404", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(false),
                ["data"] = new OpenApiNull(),
                ["message"] = new OpenApiString("Factura con id 999 no encontrada."),
                ["errors"] = new OpenApiArray()
            });

            SetResponseExample(operation, "401", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(false),
                ["data"] = new OpenApiNull(),
                ["message"] = new OpenApiString("No autorizado."),
                ["errors"] = new OpenApiArray()
            });

            return;
        }

        if (method.Equals("GET", StringComparison.OrdinalIgnoreCase) && path.Equals("/invoice/search", StringComparison.OrdinalIgnoreCase))
        {
            SetResponseExample(operation, "200", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(true),
                ["data"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["id"] = new OpenApiInteger(1),
                        ["numeroFactura"] = new OpenApiString("F-2026-0001"),
                        ["clienteNombre"] = new OpenApiString("Juan Perez"),
                        ["clienteEmail"] = new OpenApiString("juan.perez@example.com"),
                        ["fechaEmision"] = new OpenApiString("2026-02-23T10:00:00"),
                        ["fechaVencimiento"] = new OpenApiString("2026-03-05T10:00:00"),
                        ["subtotal"] = new OpenApiDouble(100.00),
                        ["impuesto"] = new OpenApiDouble(19.00),
                        ["total"] = new OpenApiDouble(119.00),
                        ["estado"] = new OpenApiString("Pendiente"),
                        ["creadoEn"] = new OpenApiString("2026-02-23T10:01:00Z")
                    }
                },
                ["message"] = new OpenApiString("Búsqueda completada."),
                ["errors"] = new OpenApiArray()
            });

            SetResponseExample(operation, "200", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(true),
                ["data"] = new OpenApiArray(),
                ["message"] = new OpenApiString("Búsqueda completada."),
                ["errors"] = new OpenApiArray()
            }, exampleKey: "SinResultados");

            SetResponseExample(operation, "400", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(false),
                ["data"] = new OpenApiNull(),
                ["message"] = new OpenApiString("El parámetro client es obligatorio."),
                ["errors"] = new OpenApiArray()
            });

            return;
        }

        if (method.Equals("PATCH", StringComparison.OrdinalIgnoreCase) && path.StartsWith("/invoice/{id}", StringComparison.OrdinalIgnoreCase) && path.EndsWith("/status", StringComparison.OrdinalIgnoreCase))
        {
            SetRequestExample(operation, new OpenApiObject
            {
                ["estado"] = new OpenApiString("Pagada")
            });

            SetResponseExample(operation, "200", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(true),
                ["data"] = new OpenApiObject
                {
                    ["id"] = new OpenApiInteger(1),
                    ["numeroFactura"] = new OpenApiString("F-2026-0001"),
                    ["clienteNombre"] = new OpenApiString("Juan Perez"),
                    ["clienteEmail"] = new OpenApiString("juan.perez@example.com"),
                    ["fechaEmision"] = new OpenApiString("2026-02-23T10:00:00"),
                    ["fechaVencimiento"] = new OpenApiString("2026-03-05T10:00:00"),
                    ["subtotal"] = new OpenApiDouble(100.00),
                    ["impuesto"] = new OpenApiDouble(19.00),
                    ["total"] = new OpenApiDouble(119.00),
                    ["estado"] = new OpenApiString("Pagada"),
                    ["creadoEn"] = new OpenApiString("2026-02-23T10:01:00Z")
                },
                ["message"] = new OpenApiString("Estado actualizado."),
                ["errors"] = new OpenApiArray()
            });

            SetResponseExample(operation, "404", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(false),
                ["data"] = new OpenApiNull(),
                ["message"] = new OpenApiString("Factura con id 999 no encontrada."),
                ["errors"] = new OpenApiArray()
            });

            SetResponseExample(operation, "400", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(false),
                ["data"] = new OpenApiNull(),
                ["message"] = new OpenApiString("Datos inválidos."),
                ["errors"] = new OpenApiArray
                {
                    new OpenApiString("Estado es obligatorio")
                }
            });

            SetResponseExample(operation, "403", new OpenApiObject
            {
                ["success"] = new OpenApiBoolean(false),
                ["data"] = new OpenApiNull(),
                ["message"] = new OpenApiString("Prohibido."),
                ["errors"] = new OpenApiArray()
            });

            return;
        }
    }

    private static void SetRequestExample(OpenApiOperation operation, IOpenApiAny example)
    {
        if (operation.RequestBody is null)
        {
            return;
        }

        if (!operation.RequestBody.Content.TryGetValue("application/json", out var content))
        {
            return;
        }

        content.Example = example;
    }

    private static void SetResponseExample(OpenApiOperation operation, string statusCode, IOpenApiAny example, string exampleKey = "Ejemplo")
    {
        if (!operation.Responses.TryGetValue(statusCode, out var response))
        {
            return;
        }

        if (!response.Content.TryGetValue("application/json", out var content))
        {
            return;
        }

        content.Examples ??= new Dictionary<string, OpenApiExample>();
        content.Examples[exampleKey] = new OpenApiExample { Value = example };
    }
}

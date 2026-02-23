using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using InvoiceService.Api.Helpers;
using InvoiceService.Api.Models;
using InvoiceService.Api.Repositories.Interfaces;

namespace InvoiceService.Api.Repositories;

public sealed class SqlInvoiceRepository : IInvoiceRepository
{
    private readonly string _connectionString;

    public SqlInvoiceRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServer") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new ApiHttpException(StatusCodes.Status500InternalServerError, "Cadena de conexión no configurada.");
        }
    }

    public async Task<Invoice?> GetByIdAsync(int id, CancellationToken ct)
    {
        await using var connection = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("SP_ObtenerFacturaPorId", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@Id", id);

        await connection.OpenAsync(ct);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
        {
            return null;
        }

        return MapInvoice(reader);
    }

    public async Task<Invoice> InsertAsync(Invoice invoice, CancellationToken ct)
    {
        await using var connection = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("SP_InsertarFactura", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@NumeroFactura", invoice.NumeroFactura);
        cmd.Parameters.AddWithValue("@ClienteNombre", invoice.ClienteNombre);
        cmd.Parameters.AddWithValue("@ClienteEmail", invoice.ClienteEmail);
        cmd.Parameters.AddWithValue("@FechaEmision", invoice.FechaEmision);
        cmd.Parameters.AddWithValue("@FechaVencimiento", invoice.FechaVencimiento);
        cmd.Parameters.AddWithValue("@Subtotal", invoice.Subtotal);
        cmd.Parameters.AddWithValue("@Impuesto", invoice.Impuesto);
        cmd.Parameters.AddWithValue("@Total", invoice.Total);
        cmd.Parameters.AddWithValue("@Estado", invoice.Estado);

        var outputId = new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(outputId);

        await connection.OpenAsync(ct);
        try
        {
            await cmd.ExecuteNonQueryAsync(ct);
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            throw new ApiHttpException(StatusCodes.Status409Conflict, "Número de factura duplicado.");
        }

        invoice.Id = (int)(outputId.Value ?? 0);
        return invoice;
    }

    public async Task<IReadOnlyList<Invoice>> SearchByClientAsync(string clientName, CancellationToken ct)
    {
        var result = new List<Invoice>();

        await using var connection = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("SP_BuscarFacturasPorCliente", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@ClienteNombre", clientName);

        await connection.OpenAsync(ct);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            result.Add(MapInvoice(reader));
        }

        return result;
    }

    public async Task<bool> UpdateStatusAsync(int id, string estado, CancellationToken ct)
    {
        await using var connection = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand("SP_ActualizarEstadoFactura", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Estado", estado);

        await connection.OpenAsync(ct);

        var returnValue = new SqlParameter
        {
            ParameterName = "@ReturnValue",
            Direction = ParameterDirection.ReturnValue
        };
        cmd.Parameters.Add(returnValue);

        await cmd.ExecuteNonQueryAsync(ct);

        var code = (int)(returnValue.Value ?? 1);
        return code == 0;
    }

    private static Invoice MapInvoice(SqlDataReader reader)
    {
        return new Invoice
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            NumeroFactura = reader.GetString(reader.GetOrdinal("NumeroFactura")),
            ClienteNombre = reader.GetString(reader.GetOrdinal("ClienteNombre")),
            ClienteEmail = reader.GetString(reader.GetOrdinal("ClienteEmail")),
            FechaEmision = reader.GetDateTime(reader.GetOrdinal("FechaEmision")),
            FechaVencimiento = reader.GetDateTime(reader.GetOrdinal("FechaVencimiento")),
            Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal")),
            Impuesto = reader.GetDecimal(reader.GetOrdinal("Impuesto")),
            Total = reader.GetDecimal(reader.GetOrdinal("Total")),
            Estado = reader.GetString(reader.GetOrdinal("Estado")),
            CreadoEn = reader.GetDateTime(reader.GetOrdinal("CreadoEn"))
        };
    }
}

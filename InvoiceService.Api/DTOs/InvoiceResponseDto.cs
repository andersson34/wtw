namespace InvoiceService.Api.DTOs;

public sealed class InvoiceResponseDto
{
    public int Id { get; init; }
    public string NumeroFactura { get; init; } = string.Empty;
    public string ClienteNombre { get; init; } = string.Empty;
    public string ClienteEmail { get; init; } = string.Empty;
    public DateTime FechaEmision { get; init; }
    public DateTime FechaVencimiento { get; init; }
    public decimal Subtotal { get; init; }
    public decimal Impuesto { get; init; }
    public decimal Total { get; init; }
    public string Estado { get; init; } = string.Empty;
    public DateTime CreadoEn { get; init; }
}

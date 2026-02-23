using InvoiceService.Api.DTOs;

namespace InvoiceService.Api.Services.Interfaces;

public interface IInvoiceService
{
    Task<InvoiceResponseDto> CreateAsync(InvoiceCreateDto dto, CancellationToken ct);
    Task<InvoiceResponseDto> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<InvoiceResponseDto>> SearchByClientAsync(string clientName, CancellationToken ct);
    Task<InvoiceResponseDto> UpdateStatusAsync(int id, string estado, CancellationToken ct);
}

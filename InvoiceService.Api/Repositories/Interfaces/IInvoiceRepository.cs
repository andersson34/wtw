using InvoiceService.Api.Models;

namespace InvoiceService.Api.Repositories.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice> InsertAsync(Invoice invoice, CancellationToken ct);
    Task<Invoice?> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<Invoice>> SearchByClientAsync(string clientName, CancellationToken ct);
    Task<bool> UpdateStatusAsync(int id, string estado, CancellationToken ct);
}

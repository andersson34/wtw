using System.Collections.Concurrent;
using InvoiceService.Api.Helpers;
using InvoiceService.Api.Models;
using InvoiceService.Api.Repositories.Interfaces;

namespace InvoiceService.Api.Repositories;

public sealed class InMemoryInvoiceRepository : IInvoiceRepository
{
    private readonly ConcurrentDictionary<int, Invoice> _storage = new();
    private int _id = 0;

    public Task<Invoice?> GetByIdAsync(int id, CancellationToken ct)
    {
        _storage.TryGetValue(id, out var invoice);
        return Task.FromResult(invoice);
    }

    public Task<Invoice> InsertAsync(Invoice invoice, CancellationToken ct)
    {
        if (_storage.Values.Any(i => string.Equals(i.NumeroFactura, invoice.NumeroFactura, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ApiHttpException(StatusCodes.Status409Conflict, "Número de factura duplicado.");
        }

        var id = Interlocked.Increment(ref _id);
        invoice.Id = id;
        invoice.CreadoEn = DateTime.UtcNow;
        _storage[id] = invoice;
        return Task.FromResult(invoice);
    }

    public Task<IReadOnlyList<Invoice>> SearchByClientAsync(string clientName, CancellationToken ct)
    {
        var result = _storage.Values
            .Where(i => i.ClienteNombre.Contains(clientName, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(i => i.FechaEmision)
            .ToList();

        return Task.FromResult<IReadOnlyList<Invoice>>(result);
    }
}

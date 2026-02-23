using AutoMapper;
using InvoiceService.Api.DTOs;
using InvoiceService.Api.Helpers;
using InvoiceService.Api.Models;
using InvoiceService.Api.Repositories.Interfaces;
using InvoiceService.Api.Services.Interfaces;

namespace InvoiceService.Api.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repo;
    private readonly IMapper _mapper;

    public InvoiceService(IInvoiceRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<InvoiceResponseDto> CreateAsync(InvoiceCreateDto dto, CancellationToken ct)
    {
        var model = _mapper.Map<Invoice>(dto);
        var created = await _repo.InsertAsync(model, ct);
        return _mapper.Map<InvoiceResponseDto>(created);
    }

    public async Task<InvoiceResponseDto> GetByIdAsync(int id, CancellationToken ct)
    {
        var invoice = await _repo.GetByIdAsync(id, ct);
        if (invoice is null)
        {
            throw new ApiHttpException(StatusCodes.Status404NotFound, $"Factura con id {id} no encontrada.");
        }

        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<IReadOnlyList<InvoiceResponseDto>> SearchByClientAsync(string clientName, CancellationToken ct)
    {
        var invoices = await _repo.SearchByClientAsync(clientName, ct);
        return invoices.Select(i => _mapper.Map<InvoiceResponseDto>(i)).ToList();
    }
}

using AutoMapper;
using InvoiceService.Api.DTOs;
using InvoiceService.Api.Models;

namespace InvoiceService.Api.Mappings;

public sealed class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<InvoiceCreateDto, Invoice>();
        CreateMap<Invoice, InvoiceResponseDto>();
    }
}

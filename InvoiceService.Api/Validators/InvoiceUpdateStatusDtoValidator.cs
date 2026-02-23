using FluentValidation;
using InvoiceService.Api.DTOs;

namespace InvoiceService.Api.Validators;

public sealed class InvoiceUpdateStatusDtoValidator : AbstractValidator<InvoiceUpdateStatusDto>
{
    public InvoiceUpdateStatusDtoValidator()
    {
        RuleFor(x => x.Estado)
            .NotEmpty()
            .Must(BeValidState);
    }

    private static bool BeValidState(string estado)
    {
        return estado is "Pendiente" or "Pagada" or "Anulada";
    }
}

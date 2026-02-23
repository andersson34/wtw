using FluentValidation;
using InvoiceService.Api.DTOs;

namespace InvoiceService.Api.Validators;

public sealed class InvoiceCreateDtoValidator : AbstractValidator<InvoiceCreateDto>
{
    public InvoiceCreateDtoValidator()
    {
        RuleFor(x => x.NumeroFactura)
            .NotEmpty();

        RuleFor(x => x.ClienteNombre)
            .NotEmpty()
            .Matches("^[^0-9]+$");

        RuleFor(x => x.ClienteEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Subtotal)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Impuesto)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Total)
            .GreaterThan(0);

        RuleFor(x => x.FechaVencimiento)
            .GreaterThan(x => x.FechaEmision);

        RuleFor(x => x.Estado)
            .NotEmpty()
            .Must(BeValidState);
    }

    private static bool BeValidState(string estado)
    {
        return estado is "Pendiente" or "Pagada" or "Anulada";
    }
}

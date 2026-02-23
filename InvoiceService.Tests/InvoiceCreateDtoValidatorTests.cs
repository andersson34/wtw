using FluentAssertions;
using InvoiceService.Api.DTOs;
using InvoiceService.Api.Validators;

namespace InvoiceService.Tests;

public sealed class InvoiceCreateDtoValidatorTests
{
    private readonly InvoiceCreateDtoValidator _validator = new();

    [Fact]
    public void Validate_WhenDtoIsValid_ShouldBeValid()
    {
        var dto = new InvoiceCreateDto
        {
            NumeroFactura = "F-0001",
            ClienteNombre = "Juan Perez",
            ClienteEmail = "juan.perez@example.com",
            FechaEmision = new DateTime(2026, 1, 1),
            FechaVencimiento = new DateTime(2026, 1, 10),
            Subtotal = 100m,
            Impuesto = 19m,
            Total = 119m,
            Estado = "Pendiente"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenClientNameHasNumbers_ShouldBeInvalid()
    {
        var dto = new InvoiceCreateDto
        {
            NumeroFactura = "F-0002",
            ClienteNombre = "Cliente123",
            ClienteEmail = "cliente@example.com",
            FechaEmision = new DateTime(2026, 1, 1),
            FechaVencimiento = new DateTime(2026, 1, 2),
            Subtotal = 10m,
            Impuesto = 0m,
            Total = 10m,
            Estado = "Pendiente"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(InvoiceCreateDto.ClienteNombre));
    }

    [Fact]
    public void Validate_WhenTotalIsZero_ShouldBeInvalid()
    {
        var dto = new InvoiceCreateDto
        {
            NumeroFactura = "F-0003",
            ClienteNombre = "Maria Gomez",
            ClienteEmail = "maria@example.com",
            FechaEmision = new DateTime(2026, 1, 1),
            FechaVencimiento = new DateTime(2026, 1, 2),
            Subtotal = 0m,
            Impuesto = 0m,
            Total = 0m,
            Estado = "Pendiente"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(InvoiceCreateDto.Total));
    }

    [Fact]
    public void Validate_WhenDueDateIsNotAfterIssueDate_ShouldBeInvalid()
    {
        var dto = new InvoiceCreateDto
        {
            NumeroFactura = "F-0004",
            ClienteNombre = "Carlos Ruiz",
            ClienteEmail = "carlos@example.com",
            FechaEmision = new DateTime(2026, 1, 2),
            FechaVencimiento = new DateTime(2026, 1, 2),
            Subtotal = 10m,
            Impuesto = 1m,
            Total = 11m,
            Estado = "Pendiente"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(InvoiceCreateDto.FechaVencimiento));
    }

    [Fact]
    public void Validate_WhenStateIsInvalid_ShouldBeInvalid()
    {
        var dto = new InvoiceCreateDto
        {
            NumeroFactura = "F-0005",
            ClienteNombre = "Ana Lopez",
            ClienteEmail = "ana@example.com",
            FechaEmision = new DateTime(2026, 1, 1),
            FechaVencimiento = new DateTime(2026, 1, 3),
            Subtotal = 10m,
            Impuesto = 1m,
            Total = 11m,
            Estado = "Borrador"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(InvoiceCreateDto.Estado));
    }
}

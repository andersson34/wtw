using AutoMapper;
using FluentAssertions;
using Moq;
using InvoiceService.Api.DTOs;
using InvoiceService.Api.Helpers;
using InvoiceService.Api.Mappings;
using InvoiceService.Api.Models;
using InvoiceService.Api.Repositories.Interfaces;
using InvoiceService.Api.Services;

namespace InvoiceService.Tests;

public sealed class InvoiceServiceTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<InvoiceProfile>());
        return config.CreateMapper();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedInvoice()
    {
        var repo = new Mock<IInvoiceRepository>(MockBehavior.Strict);
        var mapper = CreateMapper();
        var service = new InvoiceService.Api.Services.InvoiceService(repo.Object, mapper);

        var dto = new InvoiceCreateDto
        {
            NumeroFactura = "F-1000",
            ClienteNombre = "Juan Perez",
            ClienteEmail = "juan.perez@example.com",
            FechaEmision = new DateTime(2026, 1, 1),
            FechaVencimiento = new DateTime(2026, 1, 10),
            Subtotal = 100m,
            Impuesto = 19m,
            Total = 119m,
            Estado = "Pendiente"
        };

        repo.Setup(r => r.InsertAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice inv, CancellationToken _) =>
            {
                inv.Id = 1;
                inv.CreadoEn = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
                return inv;
            });

        var result = await service.CreateAsync(dto, CancellationToken.None);

        result.Id.Should().Be(1);
        result.NumeroFactura.Should().Be(dto.NumeroFactura);
        result.ClienteNombre.Should().Be(dto.ClienteNombre);
        result.Total.Should().Be(dto.Total);
        result.Estado.Should().Be(dto.Estado);

        repo.Verify(r => r.InsertAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ShouldThrow404()
    {
        var repo = new Mock<IInvoiceRepository>(MockBehavior.Strict);
        var mapper = CreateMapper();
        var service = new InvoiceService.Api.Services.InvoiceService(repo.Object, mapper);

        repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice?)null);

        var act = async () => await service.GetByIdAsync(99, CancellationToken.None);

        var ex = await act.Should().ThrowAsync<ApiHttpException>();
        ex.Which.StatusCode.Should().Be(404);

        repo.Verify(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SearchByClientAsync_WhenNoResults_ShouldReturnEmptyList()
    {
        var repo = new Mock<IInvoiceRepository>(MockBehavior.Strict);
        var mapper = CreateMapper();
        var service = new InvoiceService.Api.Services.InvoiceService(repo.Object, mapper);

        repo.Setup(r => r.SearchByClientAsync("NoExiste", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Invoice>());

        var result = await service.SearchByClientAsync("NoExiste", CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEmpty();

        repo.Verify(r => r.SearchByClientAsync("NoExiste", It.IsAny<CancellationToken>()), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SearchByClientAsync_ShouldMapInvoices()
    {
        var repo = new Mock<IInvoiceRepository>(MockBehavior.Strict);
        var mapper = CreateMapper();
        var service = new InvoiceService.Api.Services.InvoiceService(repo.Object, mapper);

        var invoices = new[]
        {
            new Invoice
            {
                Id = 10,
                NumeroFactura = "F-10",
                ClienteNombre = "Cliente Uno",
                ClienteEmail = "uno@example.com",
                FechaEmision = new DateTime(2026, 1, 1),
                FechaVencimiento = new DateTime(2026, 1, 2),
                Subtotal = 10m,
                Impuesto = 1m,
                Total = 11m,
                Estado = "Pendiente",
                CreadoEn = new DateTime(2026, 1, 1)
            }
        };

        repo.Setup(r => r.SearchByClientAsync("Cliente", It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoices);

        var result = await service.SearchByClientAsync("Cliente", CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Id.Should().Be(10);
        result[0].NumeroFactura.Should().Be("F-10");

        repo.Verify(r => r.SearchByClientAsync("Cliente", It.IsAny<CancellationToken>()), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenInvoiceExists_ShouldReturnUpdatedInvoice()
    {
        var repo = new Mock<IInvoiceRepository>(MockBehavior.Strict);
        var mapper = CreateMapper();
        var service = new InvoiceService.Api.Services.InvoiceService(repo.Object, mapper);

        repo.Setup(r => r.UpdateStatusAsync(1, "Pagada", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        repo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Invoice
            {
                Id = 1,
                NumeroFactura = "F-2026-0001",
                ClienteNombre = "Juan Perez",
                ClienteEmail = "juan.perez@example.com",
                FechaEmision = new DateTime(2026, 2, 23),
                FechaVencimiento = new DateTime(2026, 3, 5),
                Subtotal = 100m,
                Impuesto = 19m,
                Total = 119m,
                Estado = "Pagada",
                CreadoEn = new DateTime(2026, 2, 23)
            });

        var result = await service.UpdateStatusAsync(1, "Pagada", CancellationToken.None);

        result.Id.Should().Be(1);
        result.Estado.Should().Be("Pagada");

        repo.Verify(r => r.UpdateStatusAsync(1, "Pagada", It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenInvoiceDoesNotExist_ShouldThrow404()
    {
        var repo = new Mock<IInvoiceRepository>(MockBehavior.Strict);
        var mapper = CreateMapper();
        var service = new InvoiceService.Api.Services.InvoiceService(repo.Object, mapper);

        repo.Setup(r => r.UpdateStatusAsync(999, "Pagada", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await service.UpdateStatusAsync(999, "Pagada", CancellationToken.None);

        var ex = await act.Should().ThrowAsync<ApiHttpException>();
        ex.Which.StatusCode.Should().Be(404);

        repo.Verify(r => r.UpdateStatusAsync(999, "Pagada", It.IsAny<CancellationToken>()), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}

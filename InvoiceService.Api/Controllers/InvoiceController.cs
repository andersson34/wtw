using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InvoiceService.Api.DTOs;
using InvoiceService.Api.Helpers;
using InvoiceService.Api.Services.Interfaces;

namespace InvoiceService.Api.Controllers;

[ApiController]
[Route("invoice")]
[Authorize]
public sealed class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _service;

    public InvoiceController(IInvoiceService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(typeof(ApiResponse<InvoiceResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<InvoiceResponseDto>>> Create([FromBody] InvoiceCreateDto dto, CancellationToken ct)
    {
        var created = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<InvoiceResponseDto>.Ok(created, "Factura creada."));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Administrador + "," + Roles.Usuario)]
    [ProducesResponseType(typeof(ApiResponse<InvoiceResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<InvoiceResponseDto>>> GetById([FromRoute] int id, CancellationToken ct)
    {
        var invoice = await _service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<InvoiceResponseDto>.Ok(invoice, "Factura obtenida."));
    }

    [HttpGet("search")]
    [Authorize(Roles = Roles.Administrador + "," + Roles.Usuario)]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<InvoiceResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<InvoiceResponseDto>>>> Search([FromQuery(Name = "client")] string clientName, CancellationToken ct)
    {
        clientName = clientName?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(clientName))
        {
            throw new ApiHttpException(StatusCodes.Status400BadRequest, "El parámetro client es obligatorio.");
        }

        var invoices = await _service.SearchByClientAsync(clientName, ct);
        return Ok(ApiResponse<IReadOnlyList<InvoiceResponseDto>>.Ok(invoices, "Búsqueda completada."));
    }
}

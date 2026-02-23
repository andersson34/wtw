using Microsoft.AspNetCore.Mvc;
using InvoiceService.Api.DTOs;
using InvoiceService.Api.Helpers;
using InvoiceService.Api.Services.Interfaces;

namespace InvoiceService.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IJwtTokenService _jwt;

    public AuthController(IJwtTokenService jwt)
    {
        _jwt = jwt;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
    public ActionResult<ApiResponse<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new ApiHttpException(StatusCodes.Status400BadRequest, "Credenciales inválidas.");
        }

        var role = dto.Username.Equals("admin", StringComparison.OrdinalIgnoreCase)
            ? Roles.Administrador
            : Roles.Usuario;

        var token = _jwt.GenerateToken(dto.Username, role);
        return Ok(ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto { Token = token }, "Token generado."));
    }
}

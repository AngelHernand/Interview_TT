using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Interview_Base.DTOs.Common;
using Interview_Base.DTOs.User;
using Interview_Base.Models;
using Interview_Base.Services.Interfaces;

namespace Interview_Base.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    // Obtiene logs de auditoría con paginacion (solo Admin)
    [HttpGet("logs")]
    [ProducesResponseType(typeof(ApiResponseDto<PaginatedResult<AuditLog>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var result = await _auditService.GetLogsAsync(page, pageSize);
        return Ok(result);
    }

    // Obtener intentos de login con paginación (solo Admin)
    [HttpGet("login-attempts")]
    [ProducesResponseType(typeof(ApiResponseDto<PaginatedResult<LoginAttempt>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetLoginAttempts([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var result = await _auditService.GetLoginAttemptsAsync(page, pageSize);
        return Ok(result);
    }
}

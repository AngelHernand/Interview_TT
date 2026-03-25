using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Interview_Base.DTOs.Common;
using Interview_Base.DTOs.Interview;
using Interview_Base.DTOs.User;
using Interview_Base.Services.Interfaces;

namespace Interview_Base.Controllers;

[ApiController]
[Route("api/interviews")]
[Produces("application/json")]
[Authorize]
public class InterviewController : ControllerBase
{
    private readonly IInterviewService _interviewService;

    public InterviewController(IInterviewService interviewService)
    {
        _interviewService = interviewService;
    }

    //Iniciar una nueva entrevista simulada con IA
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseDto<InterviewSessionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponseDto<InterviewSessionDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> StartInterview([FromBody] StartInterviewRequestDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized(ApiResponseDto<InterviewSessionDto>.Fail("No se pudo identificar al usuario."));

        var result = await _interviewService.StartInterviewAsync(dto, userId.Value);

        if (!result.Success)
            return BadRequest(result);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    //Enviar mensaje del candidato y recibir respuesta del entrevistador
    [HttpPost("{sessionId:guid}/messages")]
    [ProducesResponseType(typeof(ApiResponseDto<InterviewMessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<InterviewMessageDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponseDto<InterviewMessageDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SendMessage(Guid sessionId, [FromBody] SendMessageRequestDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized(ApiResponseDto<InterviewMessageDto>.Fail("No se pudo identificar al usuario."));

        var result = await _interviewService.SendMessageAsync(sessionId, dto, userId.Value);

        if (!result.Success)
        {
            if (result.Message.Contains("no encontrada"))
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }

    //Finalizar entrevista y obtener evaluación
    [HttpPost("{sessionId:guid}/end")]
    [ProducesResponseType(typeof(ApiResponseDto<InterviewEvaluationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<InterviewEvaluationDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponseDto<InterviewEvaluationDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> EndInterview(Guid sessionId)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized(ApiResponseDto<InterviewEvaluationDto>.Fail("No se pudo identificar al usuario."));

        var result = await _interviewService.EndInterviewAsync(sessionId, userId.Value);

        if (!result.Success)
        {
            if (result.Message.Contains("no encontrada"))
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }

    //Obtener una sesión de entrevista con su historial de mensajes
    [HttpGet("{sessionId:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<InterviewSessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<InterviewSessionDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSession(Guid sessionId)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized(ApiResponseDto<InterviewSessionDto>.Fail("No se pudo identificar al usuario."));

        var result = await _interviewService.GetSessionAsync(sessionId, userId.Value);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    //Listar sesiones de entrevista del usuario autenticado (paginado)
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<PaginatedResult<InterviewSessionListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserSessions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized(ApiResponseDto<object>.Fail("No se pudo identificar al usuario."));

        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var result = await _interviewService.GetUserSessionsAsync(userId.Value, page, pageSize);
        return Ok(result);
    }

    // ── Helper ──────────────────────────────────────────
    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirst("userId") ??
                    User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim is not null && Guid.TryParse(claim.Value, out var id))
            return id;
        return null;
    }
}

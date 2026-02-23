using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Interview_Base.DTOs.Common;
using Interview_Base.DTOs.User;
using Interview_Base.Services.Interfaces;

namespace Interview_Base.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // Obtener datos del usuario autenticado.
    //Debe ir antes de {id} para evitar conflicto de rutas.
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Unauthorized(ApiResponseDto<object>.Fail("No se pudo identificar al usuario."));

        var result = await _userService.GetCurrentUserAsync(userId.Value);
        return result.Success ? Ok(result) : NotFound(result);
    }

    //Listar usuarios con paginación.
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<PaginatedResult<UserListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var result = await _userService.GetAllAsync(page, pageSize);
        return Ok(result);
    }

    //Obtener usuario por ID
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<UserResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    //Crear usuario (solo Admin)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponseDto<UserResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponseDto<UserResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
    {
        var result = await _userService.CreateAsync(dto);

        if (!result.Success)
            return BadRequest(result);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    //Actualizar usuario
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseDto<UserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<UserResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponseDto<UserResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDto dto)
    {
        var result = await _userService.UpdateAsync(id, dto);

        if (!result.Success)
        {
            if (result.Message.Contains("no encontrado"))
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }

    //Eliminar usuario de forma logica 
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _userService.DeleteAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    //Bloquear/desbloquear usuario (solo Admin)
    [HttpPut("{id:guid}/block")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ToggleBlock(Guid id)
    {
        var result = await _userService.ToggleBlockAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    // Helper 
    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirst("userId") ??
                    User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim is not null && Guid.TryParse(claim.Value, out var id))
            return id;
        return null;
    }
}

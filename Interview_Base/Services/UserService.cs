using Interview_Base.DTOs.Common;
using Interview_Base.DTOs.User;
using Interview_Base.Helpers;
using Interview_Base.Models;
using Interview_Base.Repositories.Interfaces;
using Interview_Base.Services.Interfaces;

namespace Interview_Base.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IAuditService _auditService;

    public UserService(IUserRepository userRepo, IAuditService auditService)
    {
        _userRepo = userRepo;
        _auditService = auditService;
    }

    // Listar con paginacion
    public async Task<ApiResponseDto<PaginatedResult<UserListDto>>> GetAllAsync(int page, int pageSize)
    {
        var (items, totalCount) = await _userRepo.GetAllPaginatedAsync(page, pageSize);

        var result = new PaginatedResult<UserListDto>
        {
            Items = items.Select(MapToListDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return ApiResponseDto<PaginatedResult<UserListDto>>.Ok(result);
    }

    //  Obtener por ID 
    public async Task<ApiResponseDto<UserResponseDto>> GetByIdAsync(Guid id)
    {
        var usuario = await _userRepo.GetByIdAsync(id);
        if (usuario is null)
            return ApiResponseDto<UserResponseDto>.Fail("Usuario no encontrado.");

        return ApiResponseDto<UserResponseDto>.Ok(MapToResponseDto(usuario));
    }

    // Crear usuario Admin 
    public async Task<ApiResponseDto<UserResponseDto>> CreateAsync(UserCreateDto dto)
    {
        if (await _userRepo.EmailExistsAsync(dto.Email))
            return ApiResponseDto<UserResponseDto>.Fail("El email ya está registrado.");

        var rol = await _userRepo.GetRolByIdAsync(dto.RolId);
        if (rol is null)
            return ApiResponseDto<UserResponseDto>.Fail("El rol especificado no existe.");

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = dto.Nombre,
            Email = dto.Email,
            PasswordHash = PasswordHelper.HashPassword(dto.Password),
            RolId = dto.RolId,
            Activo = true,
            Bloqueado = false,
            FechaCreacion = DateTime.UtcNow
        };

        await _userRepo.CreateAsync(usuario);

        await _auditService.LogAsync(null, "CREATE_USER", "Usuarios",
            usuario.Id.ToString(), null,
            System.Text.Json.JsonSerializer.Serialize(new { dto.Nombre, dto.Email, dto.RolId }));

        return ApiResponseDto<UserResponseDto>.Ok(MapToResponseDto(usuario), "Usuario creado exitosamente");
    }

    // Actualizar usuario 
    public async Task<ApiResponseDto<UserResponseDto>> UpdateAsync(Guid id, UserUpdateDto dto)
    {
        var usuario = await _userRepo.GetByIdAsync(id);
        if (usuario is null)
            return ApiResponseDto<UserResponseDto>.Fail("Usuario no encontrado.");

        // Verificar email único si cambia
        if (dto.Email is not null && dto.Email != usuario.Email)
        {
            if (await _userRepo.EmailExistsAsync(dto.Email, id))
                return ApiResponseDto<UserResponseDto>.Fail("El email ya está en uso por otro usuario.");
        }

        if (dto.RolId.HasValue)
        {
            var rol = await _userRepo.GetRolByIdAsync(dto.RolId.Value);
            if (rol is null)
                return ApiResponseDto<UserResponseDto>.Fail("El rol especificado no existe.");
        }

        var datosAnteriores = System.Text.Json.JsonSerializer.Serialize(new
        {
            usuario.Nombre, usuario.Email, usuario.RolId
        });

        // Aplicar cambios parciales
        if (dto.Nombre is not null) usuario.Nombre = dto.Nombre;
        if (dto.Email is not null) usuario.Email = dto.Email;
        if (dto.RolId.HasValue) usuario.RolId = dto.RolId.Value;

        await _userRepo.UpdateAsync(usuario);

        // Recargar el rol si cambió
        if (dto.RolId.HasValue)
        {
            usuario = await _userRepo.GetByIdAsync(id);
        }

        await _auditService.LogAsync(null, "UPDATE_USER", "Usuarios",
            id.ToString(), datosAnteriores,
            System.Text.Json.JsonSerializer.Serialize(new { dto.Nombre, dto.Email, dto.RolId }));

        return ApiResponseDto<UserResponseDto>.Ok(MapToResponseDto(usuario!), "Usuario actualizado exitosamente");
    }

    // Soft delete 
    public async Task<ApiResponseDto<object>> DeleteAsync(Guid id)
    {
        var usuario = await _userRepo.GetByIdAsync(id);
        if (usuario is null)
            return ApiResponseDto<object>.Fail("Usuario no encontrado.");

        usuario.Activo = false;
        await _userRepo.UpdateAsync(usuario);

        await _auditService.LogAsync(null, "DELETE_USER (soft)", "Usuarios", id.ToString());

        return ApiResponseDto<object>.Ok(null!, "Usuario eliminado exitosamente");
    }

    //  Usuario actual 
    public async Task<ApiResponseDto<UserResponseDto>> GetCurrentUserAsync(Guid userId)
        => await GetByIdAsync(userId);

    //  Bloquear/Desbloquear 
    public async Task<ApiResponseDto<object>> ToggleBlockAsync(Guid id)
    {
        var usuario = await _userRepo.GetByIdAsync(id);
        if (usuario is null)
            return ApiResponseDto<object>.Fail("Usuario no encontrado.");

        usuario.Bloqueado = !usuario.Bloqueado;
        await _userRepo.UpdateAsync(usuario);

        var accion = usuario.Bloqueado ? "BLOCK_USER" : "UNBLOCK_USER";
        await _auditService.LogAsync(null, accion, "Usuarios", id.ToString());

        var msg = usuario.Bloqueado ? "Usuario bloqueado" : "Usuario desbloqueado";
        return ApiResponseDto<object>.Ok(null!, msg);
    }

    // Mappers 
    private static UserResponseDto MapToResponseDto(Usuario u) => new()
    {
        Id = u.Id,
        Nombre = u.Nombre,
        Email = u.Email,
        Rol = u.Rol?.Nombre ?? "N/A",
        Activo = u.Activo,
        Bloqueado = u.Bloqueado,
        FechaCreacion = u.FechaCreacion,
        UltimoAcceso = u.UltimoAcceso
    };

    private static UserListDto MapToListDto(Usuario u) => new()
    {
        Id = u.Id,
        NombreCompleto = u.Nombre,
        Email = u.Email,
        Rol = u.Rol?.Nombre ?? "N/A",
        Activo = u.Activo,
        Bloqueado = u.Bloqueado
    };
}

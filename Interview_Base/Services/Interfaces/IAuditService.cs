using Interview_Base.DTOs.Common;
using Interview_Base.DTOs.User;
using Interview_Base.Models;

namespace Interview_Base.Services.Interfaces;

public interface IAuditService
{
    //Registra una acción de auditoría
    Task LogAsync(Guid? userId, string accion, string? entidad = null,
                  string? entidadId = null, string? valoresAnteriores = null,
                  string? valoresNuevos = null, string? ip = null);

    Task<ApiResponseDto<PaginatedResult<AuditLog>>> GetLogsAsync(int page, int pageSize);
    Task<ApiResponseDto<PaginatedResult<LoginAttempt>>> GetLoginAttemptsAsync(int page, int pageSize);
}

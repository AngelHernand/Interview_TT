using Interview_Base.DTOs.Common;
using Interview_Base.DTOs.User;
using Interview_Base.Models;
using Interview_Base.Repositories.Interfaces;
using Interview_Base.Services.Interfaces;

namespace Interview_Base.Services;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _auditRepo;

    public AuditService(IAuditRepository auditRepo)
    {
        _auditRepo = auditRepo;
    }

    
    // Registra una entrada de auditoría en la tabla AuditLogs.
    public async Task LogAsync(Guid? userId, string accion, string? entidad = null,
        string? entidadId = null, string? valoresAnteriores = null,
        string? valoresNuevos = null, string? ip = null)
    {
        var auditLog = new AuditLog
        {
            UsuarioId = userId,
            Accion = accion,
            Entidad = entidad,
            EntidadId = entidadId,
            ValoresAnteriores = valoresAnteriores,
            ValoresNuevos = valoresNuevos,
            DireccionIp = ip,
            Fecha = DateTime.UtcNow
        };

        await _auditRepo.CreateAsync(auditLog);
    }

    public async Task<ApiResponseDto<PaginatedResult<AuditLog>>> GetLogsAsync(int page, int pageSize)
    {
        var (items, totalCount) = await _auditRepo.GetLogsPaginatedAsync(page, pageSize);

        var result = new PaginatedResult<AuditLog>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return ApiResponseDto<PaginatedResult<AuditLog>>.Ok(result);
    }

    public async Task<ApiResponseDto<PaginatedResult<LoginAttempt>>> GetLoginAttemptsAsync(int page, int pageSize)
    {
        var (items, totalCount) = await _auditRepo.GetLoginAttemptsPaginatedAsync(page, pageSize);

        var result = new PaginatedResult<LoginAttempt>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return ApiResponseDto<PaginatedResult<LoginAttempt>>.Ok(result);
    }
}

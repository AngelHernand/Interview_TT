using Interview_Base.Models;

namespace Interview_Base.Repositories.Interfaces;

public interface IAuditRepository
{
    Task<AuditLog> CreateAsync(AuditLog auditLog);
    Task<(List<AuditLog> Items, int TotalCount)> GetLogsPaginatedAsync(int page, int pageSize);
    Task<(List<LoginAttempt> Items, int TotalCount)> GetLoginAttemptsPaginatedAsync(int page, int pageSize);
}

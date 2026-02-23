using Microsoft.EntityFrameworkCore;
using Interview_Base.Data;
using Interview_Base.Models;
using Interview_Base.Repositories.Interfaces;

namespace Interview_Base.Repositories;

public class AuditRepository : IAuditRepository
{
    private readonly UsersDbContext _context;

    public AuditRepository(UsersDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog> CreateAsync(AuditLog auditLog)
    {
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
        return auditLog;
    }

    public async Task<(List<AuditLog> Items, int TotalCount)> GetLogsPaginatedAsync(int page, int pageSize)
    {
        var query = _context.AuditLogs
            .Include(a => a.Usuario)
            .OrderByDescending(a => a.Fecha);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<(List<LoginAttempt> Items, int TotalCount)> GetLoginAttemptsPaginatedAsync(int page, int pageSize)
    {
        var query = _context.LoginAttempts
            .OrderByDescending(la => la.Fecha);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}

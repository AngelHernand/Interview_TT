using Microsoft.EntityFrameworkCore;
using Interview_Base.Data;
using Interview_Base.Models;
using Interview_Base.Services.Interfaces;

namespace Interview_Base.Services;

public class InterviewSessionStore : IInterviewSessionStore
{
    private readonly UsersDbContext _context;

    public InterviewSessionStore(UsersDbContext context)
    {
        _context = context;
    }

    public async Task<InterviewSession> CreateAsync(InterviewSession session)
    {
        _context.InterviewSessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<InterviewSession?> GetByIdAsync(Guid sessionId)
    {
        return await _context.InterviewSessions
            .Include(s => s.Mensajes.OrderBy(m => m.Orden))
            .FirstOrDefaultAsync(s => s.Id == sessionId);
    }

    public async Task UpdateAsync(InterviewSession session)
    {
        _context.InterviewSessions.Update(session);
        await _context.SaveChangesAsync();
    }

    public async Task AddMessageAsync(Guid sessionId, InterviewMessage message)
    {
        message.SessionId = sessionId;
        _context.InterviewMessages.Add(message);
        await _context.SaveChangesAsync();
    }

    public async Task<List<InterviewMessage>> GetMessagesAsync(Guid sessionId)
    {
        return await _context.InterviewMessages
            .Where(m => m.SessionId == sessionId)
            .OrderBy(m => m.Orden)
            .ToListAsync();
    }

    public async Task<(List<InterviewSession> Items, int TotalCount)> GetByUserPaginatedAsync(Guid userId, int page, int pageSize)
    {
        var query = _context.InterviewSessions
            .Where(s => s.UsuarioId == userId)
            .OrderByDescending(s => s.FechaInicio);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(s => s.Mensajes)
            .ToListAsync();

        return (items, total);
    }
}

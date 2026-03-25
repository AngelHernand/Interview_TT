using Interview_Base.Models;

namespace Interview_Base.Services.Interfaces;

public interface IInterviewSessionStore
{
    Task<InterviewSession> CreateAsync(InterviewSession session);
    Task<InterviewSession?> GetByIdAsync(Guid sessionId);
    Task UpdateAsync(InterviewSession session);
    Task AddMessageAsync(Guid sessionId, InterviewMessage message);
    Task<List<InterviewMessage>> GetMessagesAsync(Guid sessionId);
    Task<(List<InterviewSession> Items, int TotalCount)> GetByUserPaginatedAsync(Guid userId, int page, int pageSize);
}

using Interview_Base.DTOs.Common;
using Interview_Base.DTOs.Interview;
using Interview_Base.DTOs.User;

namespace Interview_Base.Services.Interfaces;

public interface IInterviewService
{
    Task<ApiResponseDto<InterviewSessionDto>> StartInterviewAsync(StartInterviewRequestDto request, Guid userId);
    Task<ApiResponseDto<InterviewMessageDto>> SendMessageAsync(Guid sessionId, SendMessageRequestDto request, Guid userId);
    Task<ApiResponseDto<InterviewEvaluationDto>> EndInterviewAsync(Guid sessionId, Guid userId);
    Task<ApiResponseDto<InterviewSessionDto>> GetSessionAsync(Guid sessionId, Guid userId);
    Task<ApiResponseDto<PaginatedResult<InterviewSessionListDto>>> GetUserSessionsAsync(Guid userId, int page, int pageSize);
}

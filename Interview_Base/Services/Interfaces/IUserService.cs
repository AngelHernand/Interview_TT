using Interview_Base.DTOs.Common;
using Interview_Base.DTOs.User;

namespace Interview_Base.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponseDto<PaginatedResult<UserListDto>>> GetAllAsync(int page, int pageSize);
    Task<ApiResponseDto<UserResponseDto>> GetByIdAsync(Guid id);
    Task<ApiResponseDto<UserResponseDto>> CreateAsync(UserCreateDto dto);
    Task<ApiResponseDto<UserResponseDto>> UpdateAsync(Guid id, UserUpdateDto dto);
    Task<ApiResponseDto<object>> DeleteAsync(Guid id);
    Task<ApiResponseDto<UserResponseDto>> GetCurrentUserAsync(Guid userId);
    Task<ApiResponseDto<object>> ToggleBlockAsync(Guid id);
}

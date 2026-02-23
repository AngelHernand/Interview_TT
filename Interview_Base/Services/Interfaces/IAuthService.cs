using Interview_Base.DTOs.Auth;
using Interview_Base.DTOs.Common;

namespace Interview_Base.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponseDto<LoginResponseDto>> RegisterAsync(RegisterRequestDto dto, string? ipAddress);
    Task<ApiResponseDto<LoginResponseDto>> LoginAsync(LoginRequestDto dto, string? ipAddress);
    Task<ApiResponseDto<LoginResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto dto);
    Task<ApiResponseDto<object>> LogoutAsync(RefreshTokenRequestDto dto);
    Task<ApiResponseDto<object>> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto dto);
}

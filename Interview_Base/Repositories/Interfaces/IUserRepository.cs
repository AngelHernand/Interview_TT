using Interview_Base.Models;

namespace Interview_Base.Repositories.Interfaces;

public interface IUserRepository
{
    Task<Usuario?> GetByIdAsync(Guid id);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<(List<Usuario> Items, int TotalCount)> GetAllPaginatedAsync(int page, int pageSize);
    Task<List<VwUsuariosActivo>> GetActiveUsersViewAsync();
    Task<Usuario> CreateAsync(Usuario usuario);
    Task UpdateAsync(Usuario usuario);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    Task<Rol?> GetRolByIdAsync(int rolId);
    Task<Rol?> GetRolByNameAsync(string nombre);

    // Refresh tokens
    Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task RevokeRefreshTokenAsync(RefreshToken refreshToken);
    Task RevokeAllUserRefreshTokensAsync(Guid usuarioId);

    // Login attempts
    Task<LoginAttempt> CreateLoginAttemptAsync(LoginAttempt attempt);
    Task<int> GetRecentFailedAttemptsAsync(string email, int minutesWindow);

    // Stored procedure
    Task BloquearUsuarioPorIntentosAsync(Guid usuarioId);
}

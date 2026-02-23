using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Interview_Base.Data;
using Interview_Base.Models;
using Interview_Base.Repositories.Interfaces;

namespace Interview_Base.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UsersDbContext _context;

    public UserRepository(UsersDbContext context)
    {
        _context = context;
    }

    //Usuarios 

    public async Task<Usuario?> GetByIdAsync(Guid id)
        => await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Id == id);

    public async Task<Usuario?> GetByEmailAsync(string email)
        => await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Email == email);

    public async Task<(List<Usuario> Items, int TotalCount)> GetAllPaginatedAsync(int page, int pageSize)
    {
        var query = _context.Usuarios.Include(u => u.Rol).AsQueryable();
        var total = await query.CountAsync();
        var items = await query
            .OrderBy(u => u.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<List<VwUsuariosActivo>> GetActiveUsersViewAsync()
        => await _context.VwUsuariosActivos.ToListAsync();

    public async Task<Usuario> CreateAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        // Cargar relación Rol
        await _context.Entry(usuario).Reference(u => u.Rol).LoadAsync();
        return usuario;
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null)
    {
        var query = _context.Usuarios.Where(u => u.Email == email);
        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);
        return await query.AnyAsync();
    }

    public async Task<Rol?> GetRolByIdAsync(int rolId)
        => await _context.Roles.FindAsync(rolId);

    public async Task<Rol?> GetRolByNameAsync(string nombre)
        => await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == nombre);

    // Refresh Tokens

    public async Task<RefreshToken> CreateRefreshTokenAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        => await _context.RefreshTokens
            .Include(rt => rt.Usuario)
                .ThenInclude(u => u.Rol)
            .FirstOrDefaultAsync(rt => rt.Token == token);

    public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken)
    {
        refreshToken.Revocado = true;
        refreshToken.FechaRevocacion = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllUserRefreshTokensAsync(Guid usuarioId)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UsuarioId == usuarioId && !rt.Revocado)
            .ToListAsync();

        foreach (var t in tokens)
        {
            t.Revocado = true;
            t.FechaRevocacion = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
    }

    // Login Attempts

    public async Task<LoginAttempt> CreateLoginAttemptAsync(LoginAttempt attempt)
    {
        _context.LoginAttempts.Add(attempt);
        await _context.SaveChangesAsync();
        return attempt; 
    }

    public async Task<int> GetRecentFailedAttemptsAsync(string email, int minutesWindow)
    {
        var since = DateTime.UtcNow.AddMinutes(-minutesWindow);
        return await _context.LoginAttempts
            .CountAsync(la => la.Email == email
                           && !la.Exitoso
                           && la.Fecha >= since);
    }

    //1Stored Procedure 

    public async Task BloquearUsuarioPorIntentosAsync(Guid usuarioId)
    {
        var param = new SqlParameter("@UsuarioId", usuarioId);
        await _context.Database
            .ExecuteSqlRawAsync("EXEC sp_BloquearUsuarioPorIntentos @UsuarioId", param);
    }
}

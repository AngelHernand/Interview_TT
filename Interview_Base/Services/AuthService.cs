using Interview_Base.DTOs.Auth;
using Interview_Base.DTOs.Common;
using Interview_Base.Helpers;
using Interview_Base.Models;
using Interview_Base.Repositories.Interfaces;
using Interview_Base.Services.Interfaces;

namespace Interview_Base.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IAuditService _auditService;
    private readonly JwtHelper _jwtHelper;
    private readonly IConfiguration _config;

    public AuthService(
        IUserRepository userRepo,
        IAuditService auditService,
        JwtHelper jwtHelper,
        IConfiguration config)
    {
        _userRepo = userRepo;
        _auditService = auditService;
        _jwtHelper = jwtHelper;
        _config = config;
    }

    // Register 
    public async Task<ApiResponseDto<LoginResponseDto>> RegisterAsync(
        RegisterRequestDto dto, string? ipAddress)
    {
        // Verificar email único
        if (await _userRepo.EmailExistsAsync(dto.Email))
            return ApiResponseDto<LoginResponseDto>.Fail("El email ya está registrado.");

        // Obtener rol User por defecto
        var rol = await _userRepo.GetRolByNameAsync("User");
        if (rol is null)
            return ApiResponseDto<LoginResponseDto>.Fail("Rol por defecto no encontrado. Contacte al administrador.");

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = dto.Nombre,
            Email = dto.Email,
            PasswordHash = PasswordHelper.HashPassword(dto.Password),
            RolId = rol.Id,
            Activo = true,
            Bloqueado = false,
            FechaCreacion = DateTime.UtcNow
        };

        await _userRepo.CreateAsync(usuario);

        // Generar tokens
        var token = _jwtHelper.GenerateToken(usuario.Id, usuario.Email, rol.Nombre);
        var refreshToken = await CreateRefreshTokenAsync(usuario.Id);

        await _auditService.LogAsync(usuario.Id, "REGISTER", "Usuarios",
            usuario.Id.ToString(), null, null, ipAddress);

        var response = new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken.Token,
            RedirectUrl = "/dashboard",
            Usuario = new UsuarioInfoDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = rol.Nombre
            }
        };

        return ApiResponseDto<LoginResponseDto>.Ok(response, "Registro exitoso");
    }

    //  Login 
    public async Task<ApiResponseDto<LoginResponseDto>> LoginAsync(
        LoginRequestDto dto, string? ipAddress)
    {
        var maxAttempts = int.Parse(_config["Security:MaxLoginAttempts"] ?? "5");
        var lockoutMinutes = int.Parse(_config["Security:LockoutDurationMinutes"] ?? "30");

        var usuario = await _userRepo.GetByEmailAsync(dto.Email);

        // Registrar intento
        async Task RecordAttempt(bool exitoso, string? motivo, Guid? userId)
        {
            await _userRepo.CreateLoginAttemptAsync(new LoginAttempt
            {
                Email = dto.Email,
                Exitoso = exitoso,
                DireccionIp = ipAddress,
                MensajeError = motivo,
                Fecha = DateTime.UtcNow
            });
        }

        // Validar existencia
        if (usuario is null)
        {
            await RecordAttempt(false, "Usuario no encontrado", null);
            return ApiResponseDto<LoginResponseDto>.Fail("Credenciales inválidas.");
        }

        // Validar estado
        if (!usuario.Activo)
        {
            await RecordAttempt(false, "Cuenta inactiva", usuario.Id);
            return ApiResponseDto<LoginResponseDto>.Fail("La cuenta está desactivada.");
        }

        if (usuario.Bloqueado)
        {
            await RecordAttempt(false, "Cuenta bloqueada", usuario.Id);
            return ApiResponseDto<LoginResponseDto>.Fail("La cuenta está bloqueada. Contacte al administrador.");
        }

        // Verificar contraseña
        if (!PasswordHelper.VerifyPassword(dto.Password, usuario.PasswordHash))
        {
            await RecordAttempt(false, "Contraseña incorrecta", usuario.Id);

            // Verificar si debe bloquearse
            var failedAttempts = await _userRepo.GetRecentFailedAttemptsAsync(dto.Email, lockoutMinutes);
            if (failedAttempts >= maxAttempts)
            {
                await _userRepo.BloquearUsuarioPorIntentosAsync(usuario.Id);
                return ApiResponseDto<LoginResponseDto>.Fail(
                    $"Cuenta bloqueada por exceder {maxAttempts} intentos fallidos.");
            }

            return ApiResponseDto<LoginResponseDto>.Fail("Credenciales inválidas.");
        }

        // Login exitoso actualizar ultimo acceso
        usuario.UltimoAcceso = DateTime.UtcNow;
        await _userRepo.UpdateAsync(usuario);

        await RecordAttempt(true, null, usuario.Id);

        var token = _jwtHelper.GenerateToken(usuario.Id, usuario.Email, usuario.Rol.Nombre);
        var refreshToken = await CreateRefreshTokenAsync(usuario.Id);

        await _auditService.LogAsync(usuario.Id, "LOGIN", "Usuarios",
            usuario.Id.ToString(), null, null, ipAddress);

        // Determinar redirect según rol
        var redirectUrl = usuario.Rol.Nombre switch
        {
            "Admin" => "/admin/dashboard",
            _ => "/dashboard"
        };

        var response = new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken.Token,
            RedirectUrl = redirectUrl,
            Usuario = new UsuarioInfoDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol.Nombre
            }
        };

        return ApiResponseDto<LoginResponseDto>.Ok(response, "Login exitoso");
    }

    //  Refresh Token 
    public async Task<ApiResponseDto<LoginResponseDto>> RefreshTokenAsync(
        RefreshTokenRequestDto dto)
    {
        var storedToken = await _userRepo.GetRefreshTokenAsync(dto.RefreshToken);

        if (storedToken is null || storedToken.Revocado || storedToken.FechaExpiracion < DateTime.UtcNow)
            return ApiResponseDto<LoginResponseDto>.Fail("Refresh token inválido o expirado.");

        var usuario = storedToken.Usuario;
        if (!usuario.Activo || usuario.Bloqueado)
            return ApiResponseDto<LoginResponseDto>.Fail("Usuario inactivo o bloqueado.");

        // Revocar token actual y generar uno nuevo
        await _userRepo.RevokeRefreshTokenAsync(storedToken);
        var newJwt = _jwtHelper.GenerateToken(usuario.Id, usuario.Email, usuario.Rol.Nombre);
        var newRefreshToken = await CreateRefreshTokenAsync(usuario.Id);

        var response = new LoginResponseDto
        {
            Token = newJwt,
            RefreshToken = newRefreshToken.Token,
            RedirectUrl = "/dashboard",
            Usuario = new UsuarioInfoDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol.Nombre
            }
        };

        return ApiResponseDto<LoginResponseDto>.Ok(response, "Token renovado exitosamente");
    }

    //  Logout 
    public async Task<ApiResponseDto<object>> LogoutAsync(RefreshTokenRequestDto dto)
    {
        var storedToken = await _userRepo.GetRefreshTokenAsync(dto.RefreshToken);
        if (storedToken is null)
            return ApiResponseDto<object>.Fail("Refresh token no encontrado.");

        await _userRepo.RevokeRefreshTokenAsync(storedToken);

        await _auditService.LogAsync(storedToken.UsuarioId, "LOGOUT",
            "RefreshTokens", storedToken.Id.ToString());

        return ApiResponseDto<object>.Ok(null!, "Sesión cerrada correctamente");
    }

    //  Change Password 
    public async Task<ApiResponseDto<object>> ChangePasswordAsync(
        Guid userId, ChangePasswordRequestDto dto)
    {
        var usuario = await _userRepo.GetByIdAsync(userId);
        if (usuario is null)
            return ApiResponseDto<object>.Fail("Usuario no encontrado.");

        if (!PasswordHelper.VerifyPassword(dto.CurrentPassword, usuario.PasswordHash))
            return ApiResponseDto<object>.Fail("La contraseña actual es incorrecta.");

        if (dto.NewPassword != dto.ConfirmNewPassword)
            return ApiResponseDto<object>.Fail("Las contraseñas nuevas no coinciden.");

        usuario.PasswordHash = PasswordHelper.HashPassword(dto.NewPassword);
        await _userRepo.UpdateAsync(usuario);

        // Revocar todos los refresh tokens por seguridad
        await _userRepo.RevokeAllUserRefreshTokensAsync(userId);

        await _auditService.LogAsync(userId, "CHANGE_PASSWORD", "Usuarios",
            userId.ToString());

        return ApiResponseDto<object>.Ok(null!, "Contraseña cambiada exitosamente");
    }

    // Helper privado 
    private async Task<RefreshToken> CreateRefreshTokenAsync(Guid userId)
    {
        var expirationDays = int.Parse(_config["JwtSettings:RefreshTokenExpirationDays"] ?? "7");

        var refreshToken = new RefreshToken
        {
            UsuarioId = userId,
            Token = _jwtHelper.GenerateRefreshToken(),
            FechaExpiracion = DateTime.UtcNow.AddDays(expirationDays),
            FechaCreacion = DateTime.UtcNow,
            Revocado = false
        };

        return await _userRepo.CreateRefreshTokenAsync(refreshToken);
    }
}

using FluentAssertions;
using Interview_Base.DTOs.Auth;
using Interview_Base.Helpers;
using Interview_Base.Models;
using Interview_Base.Repositories.Interfaces;
using Interview_Base.Services;
using Interview_Base.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Interview_Base.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly JwtHelper _jwtHelper;
    private readonly IConfiguration _config;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _auditServiceMock = new Mock<IAuditService>();

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = "TestSecretKey_MuySegura_AlMenos32Caracteres_12345!!",
                ["JwtSettings:Issuer"] = "TestIssuer",
                ["JwtSettings:Audience"] = "TestAudience",
                ["JwtSettings:ExpirationMinutes"] = "60",
                ["JwtSettings:RefreshTokenExpirationDays"] = "7",
                ["Security:MaxLoginAttempts"] = "5",
                ["Security:LockoutDurationMinutes"] = "30"
            })
            .Build();

        _jwtHelper = new JwtHelper(_config);

        _authService = new AuthService(
            _userRepoMock.Object,
            _auditServiceMock.Object,
            _jwtHelper,
            _config);
    }

    // ── Register ──────────────────────────────────────────

    [Fact]
    public async Task Register_EmailDuplicado_DebeRetornarFail()
    {
        _userRepoMock.Setup(r => r.EmailExistsAsync("test@test.com", null))
            .ReturnsAsync(true);

        var dto = new RegisterRequestDto
        {
            Nombre = "Test", Email = "test@test.com",
            Password = "Test123!", ConfirmPassword = "Test123!"
        };

        var result = await _authService.RegisterAsync(dto, "127.0.0.1");

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("ya está registrado");
    }

    [Fact]
    public async Task Register_RolNoEncontrado_DebeRetornarFail()
    {
        _userRepoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), null))
            .ReturnsAsync(false);
        _userRepoMock.Setup(r => r.GetRolByNameAsync("User"))
            .ReturnsAsync((Rol?)null);

        var dto = new RegisterRequestDto
        {
            Nombre = "Test", Email = "new@test.com",
            Password = "Test123!", ConfirmPassword = "Test123!"
        };

        var result = await _authService.RegisterAsync(dto, "127.0.0.1");

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Rol por defecto");
    }

    [Fact]
    public async Task Register_Exitoso_DebeRetornarTokenYUsuario()
    {
        _userRepoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), null))
            .ReturnsAsync(false);
        _userRepoMock.Setup(r => r.GetRolByNameAsync("User"))
            .ReturnsAsync(new Rol { Id = 2, Nombre = "User" });
        _userRepoMock.Setup(r => r.CreateAsync(It.IsAny<Usuario>()))
            .ReturnsAsync((Usuario u) => u);
        _userRepoMock.Setup(r => r.CreateRefreshTokenAsync(It.IsAny<RefreshToken>()))
            .ReturnsAsync((RefreshToken rt) => rt);

        var dto = new RegisterRequestDto
        {
            Nombre = "Nuevo Usuario", Email = "nuevo@test.com",
            Password = "Segura123!", ConfirmPassword = "Segura123!"
        };

        var result = await _authService.RegisterAsync(dto, "127.0.0.1");

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrWhiteSpace();
        result.Data.RefreshToken.Should().NotBeNullOrWhiteSpace();
        result.Data.Usuario.Nombre.Should().Be("Nuevo Usuario");
        result.Data.Usuario.Email.Should().Be("nuevo@test.com");
        result.Data.Usuario.Rol.Should().Be("User");
    }

    // ── Login ─────────────────────────────────────────────

    [Fact]
    public async Task Login_UsuarioNoExiste_DebeRetornarFail()
    {
        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Usuario?)null);
        _userRepoMock.Setup(r => r.CreateLoginAttemptAsync(It.IsAny<LoginAttempt>()))
            .ReturnsAsync((LoginAttempt la) => la);

        var dto = new LoginRequestDto { Email = "noexiste@test.com", Password = "Pass123!" };

        var result = await _authService.LoginAsync(dto, "127.0.0.1");

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Credenciales inválidas");
    }

    [Fact]
    public async Task Login_CuentaInactiva_DebeRetornarFail()
    {
        var usuario = CrearUsuarioMock(activo: false);
        _userRepoMock.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.CreateLoginAttemptAsync(It.IsAny<LoginAttempt>()))
            .ReturnsAsync((LoginAttempt la) => la);

        var dto = new LoginRequestDto { Email = "test@test.com", Password = "Test123!" };

        var result = await _authService.LoginAsync(dto, "127.0.0.1");

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("desactivada");
    }

    [Fact]
    public async Task Login_CuentaBloqueada_DebeRetornarFail()
    {
        var usuario = CrearUsuarioMock(bloqueado: true);
        _userRepoMock.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.CreateLoginAttemptAsync(It.IsAny<LoginAttempt>()))
            .ReturnsAsync((LoginAttempt la) => la);

        var dto = new LoginRequestDto { Email = "test@test.com", Password = "Test123!" };

        var result = await _authService.LoginAsync(dto, "127.0.0.1");

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("bloqueada");
    }

    [Fact]
    public async Task Login_PasswordIncorrecta_DebeRetornarFail()
    {
        var usuario = CrearUsuarioMock(password: "Correcta123!");
        _userRepoMock.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.CreateLoginAttemptAsync(It.IsAny<LoginAttempt>()))
            .ReturnsAsync((LoginAttempt la) => la);
        _userRepoMock.Setup(r => r.GetRecentFailedAttemptsAsync("test@test.com", 30))
            .ReturnsAsync(1);

        var dto = new LoginRequestDto { Email = "test@test.com", Password = "Incorrecta456!" };

        var result = await _authService.LoginAsync(dto, "127.0.0.1");

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Credenciales inválidas");
    }

    [Fact]
    public async Task Login_Exitoso_DebeRetornarTokenYUsuario()
    {
        var password = "MiPass123!";
        var usuario = CrearUsuarioMock(password: password, rol: "Admin");
        _userRepoMock.Setup(r => r.GetByEmailAsync("admin@test.com")).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.CreateLoginAttemptAsync(It.IsAny<LoginAttempt>()))
            .ReturnsAsync((LoginAttempt la) => la);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask);
        _userRepoMock.Setup(r => r.CreateRefreshTokenAsync(It.IsAny<RefreshToken>()))
            .ReturnsAsync((RefreshToken rt) => rt);

        var dto = new LoginRequestDto { Email = "admin@test.com", Password = password };

        var result = await _authService.LoginAsync(dto, "127.0.0.1");

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrWhiteSpace();
        result.Data.Usuario.Rol.Should().Be("Admin");
    }

    // ── Logout ────────────────────────────────────────────

    [Fact]
    public async Task Logout_TokenNoEncontrado_DebeRetornarFail()
    {
        _userRepoMock.Setup(r => r.GetRefreshTokenAsync("token-inexistente"))
            .ReturnsAsync((RefreshToken?)null);

        var dto = new RefreshTokenRequestDto { RefreshToken = "token-inexistente" };

        var result = await _authService.LogoutAsync(dto);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Logout_Exitoso_DebeRetornarOk()
    {
        var token = new RefreshToken
        {
            Id = 1, UsuarioId = Guid.NewGuid(), Token = "valid-token",
            Revocado = false, FechaExpiracion = DateTime.UtcNow.AddDays(1),
            FechaCreacion = DateTime.UtcNow
        };
        _userRepoMock.Setup(r => r.GetRefreshTokenAsync("valid-token")).ReturnsAsync(token);
        _userRepoMock.Setup(r => r.RevokeRefreshTokenAsync(token)).Returns(Task.CompletedTask);

        var dto = new RefreshTokenRequestDto { RefreshToken = "valid-token" };

        var result = await _authService.LogoutAsync(dto);

        result.Success.Should().BeTrue();
        _userRepoMock.Verify(r => r.RevokeRefreshTokenAsync(token), Times.Once);
    }

    // ── ChangePassword ────────────────────────────────────

    [Fact]
    public async Task ChangePassword_UsuarioNoEncontrado_DebeRetornarFail()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Usuario?)null);

        var dto = new ChangePasswordRequestDto
        {
            CurrentPassword = "Old123!", NewPassword = "New123!",
            ConfirmNewPassword = "New123!"
        };

        var result = await _authService.ChangePasswordAsync(Guid.NewGuid(), dto);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("no encontrado");
    }

    [Fact]
    public async Task ChangePassword_PasswordActualIncorrecta_DebeRetornarFail()
    {
        var usuario = CrearUsuarioMock(password: "Correcta123!");
        _userRepoMock.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);

        var dto = new ChangePasswordRequestDto
        {
            CurrentPassword = "Incorrecta!", NewPassword = "Nueva123!",
            ConfirmNewPassword = "Nueva123!"
        };

        var result = await _authService.ChangePasswordAsync(usuario.Id, dto);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("incorrecta");
    }

    [Fact]
    public async Task ChangePassword_Exitoso_DebeRetornarOk()
    {
        var password = "Actual123!";
        var usuario = CrearUsuarioMock(password: password);
        _userRepoMock.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask);
        _userRepoMock.Setup(r => r.RevokeAllUserRefreshTokensAsync(usuario.Id)).Returns(Task.CompletedTask);

        var dto = new ChangePasswordRequestDto
        {
            CurrentPassword = password, NewPassword = "NuevaSegura1!",
            ConfirmNewPassword = "NuevaSegura1!"
        };

        var result = await _authService.ChangePasswordAsync(usuario.Id, dto);

        result.Success.Should().BeTrue();
        _userRepoMock.Verify(r => r.RevokeAllUserRefreshTokensAsync(usuario.Id), Times.Once);
    }

    // ── Helper ────────────────────────────────────────────

    private static Usuario CrearUsuarioMock(
        string email = "test@test.com",
        string password = "Test123!",
        string rol = "User",
        bool activo = true,
        bool bloqueado = false)
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = "Test User",
            Email = email,
            PasswordHash = PasswordHelper.HashPassword(password),
            RolId = rol == "Admin" ? 1 : 2,
            Activo = activo,
            Bloqueado = bloqueado,
            FechaCreacion = DateTime.UtcNow,
            Rol = new Rol { Id = rol == "Admin" ? 1 : 2, Nombre = rol }
        };
    }
}

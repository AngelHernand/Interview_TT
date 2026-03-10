using FluentAssertions;
using Interview_Base.DTOs.User;
using Interview_Base.Helpers;
using Interview_Base.Models;
using Interview_Base.Repositories.Interfaces;
using Interview_Base.Services;
using Interview_Base.Services.Interfaces;
using Moq;

namespace Interview_Base.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _auditServiceMock = new Mock<IAuditService>();
        _userService = new UserService(_userRepoMock.Object, _auditServiceMock.Object);
    }

    // ── GetAllAsync ───────────────────────────────────────

    [Fact]
    public async Task GetAll_DebeRetornarListaPaginada()
    {
        var usuarios = new List<Usuario>
        {
            CrearUsuario("User 1", "user1@test.com"),
            CrearUsuario("User 2", "user2@test.com")
        };
        _userRepoMock.Setup(r => r.GetAllPaginatedAsync(1, 10))
            .ReturnsAsync((usuarios, 2));

        var result = await _userService.GetAllAsync(1, 10);

        result.Success.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalCount.Should().Be(2);
        result.Data.Page.Should().Be(1);
    }

    [Fact]
    public async Task GetAll_ListaVacia_DebeRetornarOk()
    {
        _userRepoMock.Setup(r => r.GetAllPaginatedAsync(1, 10))
            .ReturnsAsync((new List<Usuario>(), 0));

        var result = await _userService.GetAllAsync(1, 10);

        result.Success.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
    }

    // ── GetByIdAsync ──────────────────────────────────────

    [Fact]
    public async Task GetById_Existente_DebeRetornarUsuario()
    {
        var usuario = CrearUsuario("Test User", "test@test.com");
        _userRepoMock.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);

        var result = await _userService.GetByIdAsync(usuario.Id);

        result.Success.Should().BeTrue();
        result.Data!.Nombre.Should().Be("Test User");
        result.Data.Email.Should().Be("test@test.com");
        result.Data.Rol.Should().Be("User");
    }

    [Fact]
    public async Task GetById_NoExistente_DebeRetornarFail()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Usuario?)null);

        var result = await _userService.GetByIdAsync(Guid.NewGuid());

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("no encontrado");
    }

    // ── CreateAsync ───────────────────────────────────────

    [Fact]
    public async Task Create_EmailDuplicado_DebeRetornarFail()
    {
        _userRepoMock.Setup(r => r.EmailExistsAsync("dup@test.com", null))
            .ReturnsAsync(true);

        var dto = new UserCreateDto
        {
            Nombre = "Dup", Email = "dup@test.com",
            Password = "Pass123!", RolId = 2
        };

        var result = await _userService.CreateAsync(dto);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("ya está registrado");
    }

    [Fact]
    public async Task Create_RolNoExiste_DebeRetornarFail()
    {
        _userRepoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), null))
            .ReturnsAsync(false);
        _userRepoMock.Setup(r => r.GetRolByIdAsync(99))
            .ReturnsAsync((Rol?)null);

        var dto = new UserCreateDto
        {
            Nombre = "Test", Email = "test@test.com",
            Password = "Pass123!", RolId = 99
        };

        var result = await _userService.CreateAsync(dto);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("rol especificado no existe");
    }

    [Fact]
    public async Task Create_Exitoso_DebeRetornarUsuarioCreado()
    {
        _userRepoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), null))
            .ReturnsAsync(false);
        _userRepoMock.Setup(r => r.GetRolByIdAsync(2))
            .ReturnsAsync(new Rol { Id = 2, Nombre = "User" });
        _userRepoMock.Setup(r => r.CreateAsync(It.IsAny<Usuario>()))
            .ReturnsAsync((Usuario u) => u);

        var dto = new UserCreateDto
        {
            Nombre = "Nuevo", Email = "nuevo@test.com",
            Password = "Pass123!", RolId = 2
        };

        var result = await _userService.CreateAsync(dto);

        result.Success.Should().BeTrue();
        result.Data!.Nombre.Should().Be("Nuevo");
        result.Data.Email.Should().Be("nuevo@test.com");
        result.Message.Should().Contain("creado exitosamente");
    }

    // ── UpdateAsync ───────────────────────────────────────

    [Fact]
    public async Task Update_UsuarioNoExiste_DebeRetornarFail()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Usuario?)null);

        var result = await _userService.UpdateAsync(Guid.NewGuid(), new UserUpdateDto { Nombre = "X" });

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("no encontrado");
    }

    [Fact]
    public async Task Update_EmailDuplicado_DebeRetornarFail()
    {
        var usuario = CrearUsuario("Original", "original@test.com");
        _userRepoMock.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.EmailExistsAsync("duplicado@test.com", usuario.Id))
            .ReturnsAsync(true);

        var dto = new UserUpdateDto { Email = "duplicado@test.com" };

        var result = await _userService.UpdateAsync(usuario.Id, dto);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("ya está en uso");
    }

    [Fact]
    public async Task Update_Exitoso_DebeRetornarUsuarioActualizado()
    {
        var usuario = CrearUsuario("Antes", "antes@test.com");
        _userRepoMock.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask);

        var dto = new UserUpdateDto { Nombre = "Después" };

        var result = await _userService.UpdateAsync(usuario.Id, dto);

        result.Success.Should().BeTrue();
        result.Data!.Nombre.Should().Be("Después");
        result.Message.Should().Contain("actualizado exitosamente");
    }

    // ── DeleteAsync ───────────────────────────────────────

    [Fact]
    public async Task Delete_UsuarioNoExiste_DebeRetornarFail()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Usuario?)null);

        var result = await _userService.DeleteAsync(Guid.NewGuid());

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_Exitoso_DesactivaUsuario()
    {
        var usuario = CrearUsuario("ToDelete", "delete@test.com");
        _userRepoMock.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask);

        var result = await _userService.DeleteAsync(usuario.Id);

        result.Success.Should().BeTrue();
        usuario.Activo.Should().BeFalse();
        result.Message.Should().Contain("eliminado exitosamente");
    }

    // ── ToggleBlockAsync ──────────────────────────────────

    [Fact]
    public async Task ToggleBlock_UsuarioNoExiste_DebeRetornarFail()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Usuario?)null);

        var result = await _userService.ToggleBlockAsync(Guid.NewGuid());

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task ToggleBlock_Bloquear_DebeRetornarMensajeBloqueo()
    {
        var usuario = CrearUsuario("User", "user@test.com", bloqueado: false);
        _userRepoMock.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask);

        var result = await _userService.ToggleBlockAsync(usuario.Id);

        result.Success.Should().BeTrue();
        usuario.Bloqueado.Should().BeTrue();
        result.Message.Should().Contain("bloqueado");
    }

    [Fact]
    public async Task ToggleBlock_Desbloquear_DebeRetornarMensajeDesbloqueo()
    {
        var usuario = CrearUsuario("User", "user@test.com", bloqueado: true);
        _userRepoMock.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask);

        var result = await _userService.ToggleBlockAsync(usuario.Id);

        result.Success.Should().BeTrue();
        usuario.Bloqueado.Should().BeFalse();
        result.Message.Should().Contain("desbloqueado");
    }

    // ── Helper ────────────────────────────────────────────

    private static Usuario CrearUsuario(string nombre, string email,
        string rol = "User", bool bloqueado = false)
    {
        return new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = nombre,
            Email = email,
            PasswordHash = PasswordHelper.HashPassword("Test123!"),
            RolId = 2,
            Activo = true,
            Bloqueado = bloqueado,
            FechaCreacion = DateTime.UtcNow,
            Rol = new Rol { Id = 2, Nombre = rol }
        };
    }
}

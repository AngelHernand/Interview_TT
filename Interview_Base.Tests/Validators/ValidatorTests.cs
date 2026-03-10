using FluentAssertions;
using FluentValidation.TestHelper;
using Interview_Base.DTOs.Auth;
using Interview_Base.Validators;

namespace Interview_Base.Tests.Validators;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public void EmailVacío_DebeRetornarError()
    {
        var dto = new LoginRequestDto { Email = "", Password = "pass" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void EmailInválido_DebeRetornarError()
    {
        var dto = new LoginRequestDto { Email = "noesunemail", Password = "pass" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void PasswordVacía_DebeRetornarError()
    {
        var dto = new LoginRequestDto { Email = "test@test.com", Password = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void DatosVálidos_NoDebeRetornarErrores()
    {
        var dto = new LoginRequestDto { Email = "test@test.com", Password = "MiPass123!" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public void NombreVacío_DebeRetornarError()
    {
        var dto = new RegisterRequestDto
        {
            Nombre = "", Email = "t@t.com",
            Password = "Test123!", ConfirmPassword = "Test123!"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void NombreMuyLargo_DebeRetornarError()
    {
        var dto = new RegisterRequestDto
        {
            Nombre = new string('A', 101), Email = "t@t.com",
            Password = "Test123!", ConfirmPassword = "Test123!"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void PasswordSinMayúscula_DebeRetornarError()
    {
        var dto = new RegisterRequestDto
        {
            Nombre = "Test", Email = "t@t.com",
            Password = "test1234!", ConfirmPassword = "test1234!"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordSinNúmero_DebeRetornarError()
    {
        var dto = new RegisterRequestDto
        {
            Nombre = "Test", Email = "t@t.com",
            Password = "Testaaaa!", ConfirmPassword = "Testaaaa!"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordSinCaracterEspecial_DebeRetornarError()
    {
        var dto = new RegisterRequestDto
        {
            Nombre = "Test", Email = "t@t.com",
            Password = "Test12345", ConfirmPassword = "Test12345"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordMuyCorta_DebeRetornarError()
    {
        var dto = new RegisterRequestDto
        {
            Nombre = "Test", Email = "t@t.com",
            Password = "Te1!", ConfirmPassword = "Te1!"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void ConfirmPasswordNoCoincide_DebeRetornarError()
    {
        var dto = new RegisterRequestDto
        {
            Nombre = "Test", Email = "t@t.com",
            Password = "Test123!", ConfirmPassword = "Diferente123!"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
    }

    [Fact]
    public void DatosVálidos_NoDebeRetornarErrores()
    {
        var dto = new RegisterRequestDto
        {
            Nombre = "Test User", Email = "test@test.com",
            Password = "Segura123!", ConfirmPassword = "Segura123!"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class ChangePasswordValidatorTests
{
    private readonly ChangePasswordValidator _validator = new();

    [Fact]
    public void CurrentPasswordVacía_DebeRetornarError()
    {
        var dto = new ChangePasswordRequestDto
        {
            CurrentPassword = "",
            NewPassword = "NuevaPass1!",
            ConfirmNewPassword = "NuevaPass1!"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.CurrentPassword);
    }

    [Fact]
    public void NewPasswordSinRequisitos_DebeRetornarError()
    {
        var dto = new ChangePasswordRequestDto
        {
            CurrentPassword = "Actual123!",
            NewPassword = "corta",
            ConfirmNewPassword = "corta"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void ConfirmNewPasswordNoCoincide_DebeRetornarError()
    {
        var dto = new ChangePasswordRequestDto
        {
            CurrentPassword = "Actual123!",
            NewPassword = "NuevaPass1!",
            ConfirmNewPassword = "OtraPass1!"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword);
    }

    [Fact]
    public void DatosVálidos_NoDebeRetornarErrores()
    {
        var dto = new ChangePasswordRequestDto
        {
            CurrentPassword = "Actual123!",
            NewPassword = "NuevaSegura1!",
            ConfirmNewPassword = "NuevaSegura1!"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public class UserCreateValidatorTests
{
    private readonly UserCreateValidator _validator = new();

    [Fact]
    public void RolIdCero_DebeRetornarError()
    {
        var dto = new Interview_Base.DTOs.User.UserCreateDto
        {
            Nombre = "Test", Email = "t@t.com",
            Password = "Test123!", RolId = 0
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.RolId);
    }

    [Fact]
    public void DatosVálidos_NoDebeRetornarErrores()
    {
        var dto = new Interview_Base.DTOs.User.UserCreateDto
        {
            Nombre = "Admin", Email = "admin@test.com",
            Password = "Admin123!", RolId = 1
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

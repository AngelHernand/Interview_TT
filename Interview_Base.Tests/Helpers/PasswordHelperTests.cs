using FluentAssertions;
using Interview_Base.Helpers;

namespace Interview_Base.Tests.Helpers;

public class PasswordHelperTests
{
    [Fact]
    public void HashPassword_DebeGenerarHashNoVacio()
    {
        var hash = PasswordHelper.HashPassword("MiPassword123!");

        hash.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void HashPassword_DebeGenerarHashBCryptValido()
    {
        var hash = PasswordHelper.HashPassword("Test123!");

        hash.Should().StartWith("$2a$11$");
    }

    [Fact]
    public void HashPassword_MismaPasswordGeneraHashesDiferentes()
    {
        var hash1 = PasswordHelper.HashPassword("MiPassword123!");
        var hash2 = PasswordHelper.HashPassword("MiPassword123!");

        hash1.Should().NotBe(hash2, "BCrypt genera un salt distinto cada vez");
    }

    [Fact]
    public void VerifyPassword_PasswordCorrecta_DebeRetornarTrue()
    {
        var password = "Segura123!";
        var hash = PasswordHelper.HashPassword(password);

        PasswordHelper.VerifyPassword(password, hash).Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_PasswordIncorrecta_DebeRetornarFalse()
    {
        var hash = PasswordHelper.HashPassword("Correcta123!");

        PasswordHelper.VerifyPassword("Incorrecta456!", hash).Should().BeFalse();
    }

    [Theory]
    [InlineData("a")]
    [InlineData("ContraseñaMuyLarga1234567890!@#$%^&*()")]
    [InlineData("Ñandú_2024!")]
    public void HashPassword_DiversasPasswords_DebeVerificarCorrectamente(string password)
    {
        var hash = PasswordHelper.HashPassword(password);

        PasswordHelper.VerifyPassword(password, hash).Should().BeTrue();
    }
}

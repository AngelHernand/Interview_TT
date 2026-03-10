using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Interview_Base.Helpers;
using Microsoft.Extensions.Configuration;

namespace Interview_Base.Tests.Helpers;

public class JwtHelperTests
{
    private readonly JwtHelper _jwtHelper;

    public JwtHelperTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = "TestSecretKey_MuySegura_AlMenos32Caracteres_12345!!",
                ["JwtSettings:Issuer"] = "TestIssuer",
                ["JwtSettings:Audience"] = "TestAudience",
                ["JwtSettings:ExpirationMinutes"] = "60",
                ["JwtSettings:RefreshTokenExpirationDays"] = "7"
            })
            .Build();

        _jwtHelper = new JwtHelper(config);
    }

    [Fact]
    public void GenerateToken_DebeRetornarTokenNoVacio()
    {
        var userId = Guid.NewGuid();

        var token = _jwtHelper.GenerateToken(userId, "test@test.com", "Admin");

        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateToken_DebeContenerClaimsCorrectos()
    {
        var userId = Guid.NewGuid();
        var email = "admin@sistema.com";
        var role = "Admin";

        var token = _jwtHelper.GenerateToken(userId, email, role);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Claims.Should().Contain(c => c.Type == "email" && c.Value == email);
        jwtToken.Claims.Should().Contain(c => c.Type == "userId" && c.Value == userId.ToString());
        jwtToken.Claims.Should().Contain(c =>
            c.Type == ClaimTypes.Role && c.Value == role);
    }

    [Fact]
    public void GenerateToken_DebeExpirarEnElFuturo()
    {
        var token = _jwtHelper.GenerateToken(Guid.NewGuid(), "test@test.com", "User");

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.ValidTo.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void GenerateToken_DebeContenerIssuerYAudienceCorrectos()
    {
        var token = _jwtHelper.GenerateToken(Guid.NewGuid(), "test@test.com", "User");

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Issuer.Should().Be("TestIssuer");
        jwtToken.Audiences.Should().Contain("TestAudience");
    }

    [Fact]
    public void GenerateRefreshToken_DebeRetornarStringBase64NoVacio()
    {
        var refreshToken = _jwtHelper.GenerateRefreshToken();

        refreshToken.Should().NotBeNullOrWhiteSpace();
        // Verificar que es base64 válido
        var act = () => Convert.FromBase64String(refreshToken);
        act.Should().NotThrow();
    }

    [Fact]
    public void GenerateRefreshToken_DebeGenerarTokensUnicos()
    {
        var token1 = _jwtHelper.GenerateRefreshToken();
        var token2 = _jwtHelper.GenerateRefreshToken();

        token1.Should().NotBe(token2);
    }

    [Fact]
    public void ValidateToken_TokenValido_DebeRetornarClaimsPrincipal()
    {
        var userId = Guid.NewGuid();
        var token = _jwtHelper.GenerateToken(userId, "test@test.com", "Admin");

        var principal = _jwtHelper.ValidateToken(token);

        principal.Should().NotBeNull();
    }

    [Fact]
    public void ValidateToken_TokenInvalido_DebeRetornarNull()
    {
        var result = _jwtHelper.ValidateToken("token.invalido.aqui");

        result.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_TokenVacio_DebeRetornarNull()
    {
        var result = _jwtHelper.ValidateToken("");

        result.Should().BeNull();
    }
}

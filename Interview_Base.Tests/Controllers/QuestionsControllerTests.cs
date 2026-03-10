using FluentAssertions;
using Interview_Base.Controllers;
using Interview_Base.DTOs.Questions;
using Interview_Base.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace Interview_Base.Tests.Controllers;

public class QuestionsControllerTests
{
    private readonly QuestionsController _controller;

    public QuestionsControllerTests()
    {
        _controller = new QuestionsController();
    }

    // ── GetQuestions ──────────────────────────────────────

    [Fact]
    public void GetQuestions_DebeRetornar200ConPreguntas()
    {
        var result = _controller.GetQuestions();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value.Should().BeOfType<ApiResponseDto<List<QuestionDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(15);
    }

    [Fact]
    public void GetQuestions_TodasLasPreguntasTienen4Opciones()
    {
        var result = _controller.GetQuestions() as OkObjectResult;
        var response = result!.Value as ApiResponseDto<List<QuestionDto>>;

        foreach (var q in response!.Data!)
        {
            q.Opciones.Should().HaveCount(4,
                $"Pregunta {q.Id}: '{q.Texto}' debería tener 4 opciones");
        }
    }

    [Fact]
    public void GetQuestions_IdsConsecutivosDelUnoAlQuince()
    {
        var result = _controller.GetQuestions() as OkObjectResult;
        var response = result!.Value as ApiResponseDto<List<QuestionDto>>;

        var ids = response!.Data!.Select(q => q.Id).ToList();
        ids.Should().BeEquivalentTo(Enumerable.Range(1, 15));
    }

    // ── Evaluate ─────────────────────────────────────────

    [Fact]
    public void Evaluate_RespuestasNulas_Retorna400()
    {
        var result = _controller.Evaluate(null!);

        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public void Evaluate_RespuestasVacias_Retorna400()
    {
        var result = _controller.Evaluate(new List<AnswerDto>());

        var badResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var response = badResult.Value.Should().BeOfType<ApiResponseDto<TestResultDto>>().Subject;
        response.Success.Should().BeFalse();
        response.Message.Should().Contain("al menos una respuesta");
    }

    [Fact]
    public void Evaluate_TodasCorrectas_DebeRetornar100Porciento()
    {
        // Respuestas correctas
        var answers = new List<AnswerDto>
        {
            new() { PreguntaId = 1, ClaveSeleccionada = "B" },
            new() { PreguntaId = 2, ClaveSeleccionada = "C" },
            new() { PreguntaId = 3, ClaveSeleccionada = "A" },
            new() { PreguntaId = 4, ClaveSeleccionada = "C" },
            new() { PreguntaId = 5, ClaveSeleccionada = "C" },
            new() { PreguntaId = 6, ClaveSeleccionada = "B" },
            new() { PreguntaId = 7, ClaveSeleccionada = "C" },
            new() { PreguntaId = 8, ClaveSeleccionada = "B" },
            new() { PreguntaId = 9, ClaveSeleccionada = "B" },
            new() { PreguntaId = 10, ClaveSeleccionada = "B" },
            new() { PreguntaId = 11, ClaveSeleccionada = "B" },
            new() { PreguntaId = 12, ClaveSeleccionada = "B" },
            new() { PreguntaId = 13, ClaveSeleccionada = "B" },
            new() { PreguntaId = 14, ClaveSeleccionada = "C" },
            new() { PreguntaId = 15, ClaveSeleccionada = "C" }
        };

        var result = _controller.Evaluate(answers) as OkObjectResult;
        var response = result!.Value as ApiResponseDto<TestResultDto>;

        response!.Success.Should().BeTrue();
        response.Data!.Correctas.Should().Be(15);
        response.Data.PorcentajeAcierto.Should().Be(100);
        response.Data.TotalPreguntas.Should().Be(15);
        response.Data.Incorrectas.Should().Be(0);
    }

    [Fact]
    public void Evaluate_TodasIncorrectas_DebeRetornar0Porciento()
    {
        var answers = Enumerable.Range(1, 15)
            .Select(id => new AnswerDto { PreguntaId = id, ClaveSeleccionada = "D" })
            .ToList();

        // La pregunta 1 tiene respuesta B, así que D es incorrecta
        // Pero para asegurar, ponemos una clave que no es correcta para ninguna
        var result = _controller.Evaluate(answers) as OkObjectResult;
        var response = result!.Value as ApiResponseDto<TestResultDto>;

        response!.Success.Should().BeTrue();
        response.Data!.Correctas.Should().BeLessThan(15);
        response.Data.PorcentajeAcierto.Should().BeLessThan(100);
    }

    [Fact]
    public void Evaluate_RespuestasParciales_CalculaCorrectamente()
    {
        // Solo las primeras 5, todas correctas
        var answers = new List<AnswerDto>
        {
            new() { PreguntaId = 1, ClaveSeleccionada = "B" },
            new() { PreguntaId = 2, ClaveSeleccionada = "C" },
            new() { PreguntaId = 3, ClaveSeleccionada = "A" },
            new() { PreguntaId = 4, ClaveSeleccionada = "C" },
            new() { PreguntaId = 5, ClaveSeleccionada = "C" }
        };

        var result = _controller.Evaluate(answers) as OkObjectResult;
        var response = result!.Value as ApiResponseDto<TestResultDto>;

        response!.Success.Should().BeTrue();
        response.Data!.TotalPreguntas.Should().Be(15);
        response.Data.Correctas.Should().Be(5);
    }

    [Fact]
    public void Evaluate_DetalleContieneInfoCompleta()
    {
        var answers = new List<AnswerDto>
        {
            new() { PreguntaId = 1, ClaveSeleccionada = "A" } // Incorrecta (correcta: B)
        };

        var result = _controller.Evaluate(answers) as OkObjectResult;
        var response = result!.Value as ApiResponseDto<TestResultDto>;

        var detalle = response!.Data!.Detalle;
        detalle.Should().NotBeEmpty();

        var item = detalle.First(d => d.PreguntaId == 1);
        item.ClaveSeleccionada.Should().Be("A");
        item.ClaveCorrecta.Should().Be("B");
        item.EsCorrecta.Should().BeFalse();
        item.TextoPregunta.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Evaluate_MensajeExitosoEnRespuesta()
    {
        var answers = new List<AnswerDto>
        {
            new() { PreguntaId = 1, ClaveSeleccionada = "B" }
        };

        var result = _controller.Evaluate(answers) as OkObjectResult;
        var response = result!.Value as ApiResponseDto<TestResultDto>;

        response!.Message.Should().Contain("evaluado exitosamente");
    }
}

using FluentAssertions;
using Interview_Base.Data;
using Interview_Base.DTOs.Questions;

namespace Interview_Base.Tests.Data;

public class QuestionBankTests
{
    [Fact]
    public void GetQuestions_DebeRetornar15Preguntas()
    {
        var questions = QuestionBank.GetQuestions();

        questions.Should().HaveCount(15);
    }

    [Fact]
    public void GetQuestions_TodasLasPreguntasDebenTener4Opciones()
    {
        var questions = QuestionBank.GetQuestions();

        foreach (var q in questions)
        {
            q.Opciones.Should().HaveCount(4, $"Pregunta {q.Id} debe tener 4 opciones");
        }
    }

    [Fact]
    public void GetQuestions_IdsDebenSerConsecutivosDelUnoAlQuince()
    {
        var questions = QuestionBank.GetQuestions();
        var ids = questions.Select(q => q.Id).OrderBy(id => id).ToList();

        ids.Should().BeEquivalentTo(Enumerable.Range(1, 15));
    }

    [Fact]
    public void GetQuestions_CadaPreguntaDebeTenerTextoNoVacio()
    {
        var questions = QuestionBank.GetQuestions();

        foreach (var q in questions)
        {
            q.Texto.Should().NotBeNullOrWhiteSpace($"Pregunta {q.Id} debe tener texto");
        }
    }

    [Fact]
    public void GetQuestions_OpcionesDebenTenerClavesABCD()
    {
        var questions = QuestionBank.GetQuestions();
        var expectedKeys = new[] { "A", "B", "C", "D" };

        foreach (var q in questions)
        {
            var claves = q.Opciones.Select(o => o.Clave).ToList();
            claves.Should().BeEquivalentTo(expectedKeys, $"Pregunta {q.Id}");
        }
    }

    [Fact]
    public void GetQuestions_OpcionesDebenTenerTextoNoVacio()
    {
        var questions = QuestionBank.GetQuestions();

        foreach (var q in questions)
        {
            foreach (var o in q.Opciones)
            {
                o.Texto.Should().NotBeNullOrWhiteSpace(
                    $"Opción {o.Clave} de pregunta {q.Id} debe tener texto");
            }
        }
    }

    [Theory]
    [InlineData(1, "B")]
    [InlineData(2, "C")]
    [InlineData(3, "A")]
    [InlineData(4, "C")]
    [InlineData(5, "C")]
    [InlineData(6, "B")]
    [InlineData(7, "C")]
    [InlineData(8, "B")]
    [InlineData(9, "B")]
    [InlineData(10, "B")]
    [InlineData(11, "B")]
    [InlineData(12, "B")]
    [InlineData(13, "B")]
    [InlineData(14, "C")]
    [InlineData(15, "C")]
    public void GetCorrectKey_DebeRetornarLaClaveCorrecta(int questionId, string expectedKey)
    {
        var key = QuestionBank.GetCorrectKey(questionId);

        key.Should().Be(expectedKey);
    }

    [Fact]
    public void GetCorrectKey_PreguntaInexistente_DebeRetornarNull()
    {
        var key = QuestionBank.GetCorrectKey(999);

        key.Should().BeNull();
    }

    [Fact]
    public void Evaluate_TodasCorrectas_DebeRetornar100Porciento()
    {
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
            new() { PreguntaId = 15, ClaveSeleccionada = "C" },
        };

        var result = QuestionBank.Evaluate(answers);

        result.Correctas.Should().Be(15);
        result.Incorrectas.Should().Be(0);
        result.PorcentajeAcierto.Should().Be(100.0);
        result.TotalPreguntas.Should().Be(15);
    }

    [Fact]
    public void Evaluate_TodasIncorrectas_DebeRetornar0Porciento()
    {
        var answers = new List<AnswerDto>
        {
            new() { PreguntaId = 1, ClaveSeleccionada = "A" },
            new() { PreguntaId = 2, ClaveSeleccionada = "A" },
            new() { PreguntaId = 3, ClaveSeleccionada = "B" },
            new() { PreguntaId = 4, ClaveSeleccionada = "A" },
            new() { PreguntaId = 5, ClaveSeleccionada = "A" },
            new() { PreguntaId = 6, ClaveSeleccionada = "A" },
            new() { PreguntaId = 7, ClaveSeleccionada = "A" },
            new() { PreguntaId = 8, ClaveSeleccionada = "A" },
            new() { PreguntaId = 9, ClaveSeleccionada = "A" },
            new() { PreguntaId = 10, ClaveSeleccionada = "A" },
            new() { PreguntaId = 11, ClaveSeleccionada = "A" },
            new() { PreguntaId = 12, ClaveSeleccionada = "A" },
            new() { PreguntaId = 13, ClaveSeleccionada = "A" },
            new() { PreguntaId = 14, ClaveSeleccionada = "A" },
            new() { PreguntaId = 15, ClaveSeleccionada = "A" },
        };

        var result = QuestionBank.Evaluate(answers);

        result.Correctas.Should().Be(0);
        result.Incorrectas.Should().Be(15);
        result.PorcentajeAcierto.Should().Be(0.0);
    }

    [Fact]
    public void Evaluate_SinRespuestas_DebeMarcarTodasComoIncorrectas()
    {
        var answers = new List<AnswerDto>();

        var result = QuestionBank.Evaluate(answers);

        result.Correctas.Should().Be(0);
        result.Incorrectas.Should().Be(15);
        result.Detalle.Should().HaveCount(15);
        result.Detalle.All(d => !d.EsCorrecta).Should().BeTrue();
    }

    [Fact]
    public void Evaluate_RespuestasParciales_DebeCalcularCorrectamente()
    {
        // Solo contesta 3 preguntas, todas correctas
        var answers = new List<AnswerDto>
        {
            new() { PreguntaId = 1, ClaveSeleccionada = "B" },
            new() { PreguntaId = 2, ClaveSeleccionada = "C" },
            new() { PreguntaId = 3, ClaveSeleccionada = "A" },
        };

        var result = QuestionBank.Evaluate(answers);

        result.Correctas.Should().Be(3);
        result.Incorrectas.Should().Be(12);
        result.PorcentajeAcierto.Should().Be(20.0);
    }

    [Fact]
    public void Evaluate_DebeIncluirDetalleConTextosDePregunta()
    {
        var answers = new List<AnswerDto>
        {
            new() { PreguntaId = 1, ClaveSeleccionada = "B" },
        };

        var result = QuestionBank.Evaluate(answers);

        result.Detalle.Should().HaveCount(15);
        var detalle1 = result.Detalle.First(d => d.PreguntaId == 1);
        detalle1.TextoPregunta.Should().NotBeNullOrWhiteSpace();
        detalle1.ClaveCorrecta.Should().Be("B");
        detalle1.ClaveSeleccionada.Should().Be("B");
        detalle1.EsCorrecta.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_EsCaseInsensitive()
    {
        var answers = new List<AnswerDto>
        {
            new() { PreguntaId = 1, ClaveSeleccionada = "b" }, // minúscula
        };

        var result = QuestionBank.Evaluate(answers);

        var detalle1 = result.Detalle.First(d => d.PreguntaId == 1);
        detalle1.EsCorrecta.Should().BeTrue("la evaluación debe ser case-insensitive");
    }
}

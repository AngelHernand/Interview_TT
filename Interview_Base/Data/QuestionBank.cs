using Interview_Base.DTOs.Questions;

namespace Interview_Base.Data;

/// <summary>
/// Fuente de datos estática para las preguntas del test.
/// Desacoplada del controlador para que en el futuro se pueda
/// reemplazar por un servicio externo, base de datos, etc.
/// </summary>
public static class QuestionBank
{
    private static readonly List<(QuestionDto Question, string CorrectKey)> _questions = new()
    {
        (new QuestionDto
        {
            Id = 1,
            Texto = "¿Cuál es el lenguaje principal de ASP.NET Core?",
            Opciones = new()
            {
                new() { Clave = "A", Texto = "Java" },
                new() { Clave = "B", Texto = "C#" },
                new() { Clave = "C", Texto = "Python" },
                new() { Clave = "D", Texto = "Ruby" },
            }
        }, "B"),

        (new QuestionDto
        {
            Id = 2,
            Texto = "¿Qué patrón de diseño usa la inyección de dependencias?",
            Opciones = new()
            {
                new() { Clave = "A", Texto = "Singleton" },
                new() { Clave = "B", Texto = "Factory" },
                new() { Clave = "C", Texto = "Inversión de Control (IoC)" },
                new() { Clave = "D", Texto = "Observer" },
            }
        }, "C"),

        (new QuestionDto
        {
            Id = 3,
            Texto = "¿Qué significa ORM en el contexto de Entity Framework?",
            Opciones = new()
            {
                new() { Clave = "A", Texto = "Object Relational Mapping" },
                new() { Clave = "B", Texto = "Object Resource Management" },
                new() { Clave = "C", Texto = "Online Request Model" },
                new() { Clave = "D", Texto = "Operational Risk Module" },
            }
        }, "A"),

        (new QuestionDto
        {
            Id = 4,
            Texto = "¿Cuál de los siguientes NO es un método HTTP estándar?",
            Opciones = new()
            {
                new() { Clave = "A", Texto = "GET" },
                new() { Clave = "B", Texto = "POST" },
                new() { Clave = "C", Texto = "SEND" },
                new() { Clave = "D", Texto = "PUT" },
            }
        }, "C"),

        (new QuestionDto
        {
            Id = 5,
            Texto = "¿Qué status code HTTP indica 'No encontrado'?",
            Opciones = new()
            {
                new() { Clave = "A", Texto = "200" },
                new() { Clave = "B", Texto = "301" },
                new() { Clave = "C", Texto = "404" },
                new() { Clave = "D", Texto = "500" },
            }
        }, "C"),

        (new QuestionDto
        {
            Id = 6,
            Texto = "¿Qué tipo de base de datos es SQL Server?",
            Opciones = new()
            {
                new() { Clave = "A", Texto = "NoSQL" },
                new() { Clave = "B", Texto = "Relacional" },
                new() { Clave = "C", Texto = "Grafos" },
                new() { Clave = "D", Texto = "Clave-Valor" },
            }
        }, "B"),

        (new QuestionDto
        {
            Id = 7,
            Texto = "¿Qué palabra clave se usa en C# para la herencia?",
            Opciones = new()
            {
                new() { Clave = "A", Texto = "implements" },
                new() { Clave = "B", Texto = "extends" },
                new() { Clave = "C", Texto = ":" },
                new() { Clave = "D", Texto = "inherits" },
            }
        }, "C"),

        (new QuestionDto
        {
            Id = 8,
            Texto = "¿Cuál es la función de un middleware en ASP.NET Core?",
            Opciones = new()
            {
                new() { Clave = "A", Texto = "Compilar el código" },
                new() { Clave = "B", Texto = "Procesar solicitudes HTTP en un pipeline" },
                new() { Clave = "C", Texto = "Gestionar la base de datos" },
                new() { Clave = "D", Texto = "Renderizar vistas HTML" },
            }
        }, "B"),

        (new QuestionDto
        {
            Id = 9,
            Texto = "¿Qué es JWT?",
            Opciones = new()
            {
                new() { Clave = "A", Texto = "Java Web Token" },
                new() { Clave = "B", Texto = "JSON Web Token" },
                new() { Clave = "C", Texto = "JavaScript Web Transfer" },
                new() { Clave = "D", Texto = "JSON Web Transfer" },
            }
        }, "B"),

        (new QuestionDto
        {
            Id = 10,
            Texto = "¿Cuál de las siguientes es una característica de REST?",
            Opciones = new()
            {
                new() { Clave = "A", Texto = "Con estado (stateful)" },
                new() { Clave = "B", Texto = "Sin estado (stateless)" },
                new() { Clave = "C", Texto = "Requiere WebSocket" },
                new() { Clave = "D", Texto = "Solo funciona con XML" },
            }
        }, "B"),

        (new QuestionDto
        {
            Id = 11,
            Texto = "¿Qué operador de LINQ se usa para filtrar elementos?",
            Opciones = new()
            {
                new() { Clave = "A", Texto = "Select" },
                new() { Clave = "B", Texto = "Where" },
                new() { Clave = "C", Texto = "OrderBy" },
                new() { Clave = "D", Texto = "GroupBy" },
            }
        }, "B"),

        (new QuestionDto
        {
            Id = 12,
            Texto = "¿Qué patrón separa la lógica de negocio de la persistencia?",
            Opciones = new()
            {
                new() { Clave = "A", Texto = "MVC" },
                new() { Clave = "B", Texto = "Repository" },
                new() { Clave = "C", Texto = "Observer" },
                new() { Clave = "D", Texto = "Decorator" },
            }
        }, "B"),

        (new QuestionDto
        {
            Id = 13,
            Texto = "¿Qué hace 'async/await' en C#?",
            Opciones = new()
            {
                new() { Clave = "A", Texto = "Crea hilos nuevos" },
                new() { Clave = "B", Texto = "Permite programación asíncrona sin bloquear el hilo" },
                new() { Clave = "C", Texto = "Ejecuta código en paralelo obligatoriamente" },
                new() { Clave = "D", Texto = "Solo funciona con bases de datos" },
            }
        }, "B"),

        (new QuestionDto
        {
            Id = 14,
            Texto = "¿Qué tipo de dato devuelve una tarea asíncrona en C#?",
            Opciones = new()
            {
                new() { Clave = "A", Texto = "void" },
                new() { Clave = "B", Texto = "Thread" },
                new() { Clave = "C", Texto = "Task" },
                new() { Clave = "D", Texto = "Promise" },
            }
        }, "C"),

        (new QuestionDto
        {
            Id = 15,
            Texto = "¿Cuál es la ventaja principal de usar DTOs?",
            Opciones = new()
            {
                new() { Clave = "A", Texto = "Aumentar el acoplamiento" },
                new() { Clave = "B", Texto = "Exponer directamente las entidades de BD" },
                new() { Clave = "C", Texto = "Controlar qué datos se exponen y transferir solo lo necesario" },
                new() { Clave = "D", Texto = "Eliminar la necesidad de validaciones" },
            }
        }, "C"),
    };

    /// <summary>Obtiene las 15 preguntas (sin la respuesta correcta).</summary>
    public static List<QuestionDto> GetQuestions()
    {
        return _questions.Select(q => q.Question).ToList();
    }

    /// <summary>Obtiene la clave correcta de una pregunta por su Id.</summary>
    public static string? GetCorrectKey(int questionId)
    {
        var item = _questions.FirstOrDefault(q => q.Question.Id == questionId);
        return item == default ? null : item.CorrectKey;
    }

    /// <summary>Evalúa las respuestas y devuelve el resultado.</summary>
    public static TestResultDto Evaluate(List<AnswerDto> answers)
    {
        var result = new TestResultDto
        {
            TotalPreguntas = _questions.Count,
            Detalle = new()
        };

        foreach (var q in _questions)
        {
            var answer = answers.FirstOrDefault(a => a.PreguntaId == q.Question.Id);
            var selected = answer?.ClaveSeleccionada ?? "";
            var isCorrect = selected.Equals(q.CorrectKey, StringComparison.OrdinalIgnoreCase);

            if (isCorrect) result.Correctas++;
            else result.Incorrectas++;

            result.Detalle.Add(new QuestionResultDto
            {
                PreguntaId = q.Question.Id,
                TextoPregunta = q.Question.Texto,
                ClaveSeleccionada = selected,
                ClaveCorrecta = q.CorrectKey,
                EsCorrecta = isCorrect
            });
        }

        result.PorcentajeAcierto = Math.Round((double)result.Correctas / result.TotalPreguntas * 100, 1);
        return result;
    }
}

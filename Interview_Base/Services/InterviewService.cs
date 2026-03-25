using System.Text.Json;
using Interview_Base.DTOs.Common;
using Interview_Base.DTOs.Interview;
using Interview_Base.DTOs.User;
using Interview_Base.Models;
using Interview_Base.Models.Interview;
using Interview_Base.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Interview_Base.Services;

public class InterviewService : IInterviewService
{
    private readonly IOllamaClient _ollama;
    private readonly IQdrantSearchClient _qdrant;
    private readonly IBehavioralQuestionProvider _behavioral;
    private readonly IInterviewSessionStore _store;
    private readonly IAuditService _audit;
    private readonly IConfiguration _config;
    private readonly ILogger<InterviewService> _logger;

    private static readonly HashSet<string> TiposValidos = new(StringComparer.OrdinalIgnoreCase)
        { "tecnica", "behavioral", "mixta" };

    private static readonly HashSet<string> NivelesValidos = new(StringComparer.OrdinalIgnoreCase)
        { "Junior", "Mid", "Senior" };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public InterviewService(
        IOllamaClient ollama,
        IQdrantSearchClient qdrant,
        IBehavioralQuestionProvider behavioral,
        IInterviewSessionStore store,
        IAuditService audit,
        IConfiguration config,
        ILogger<InterviewService> logger)
    {
        _ollama = ollama;
        _qdrant = qdrant;
        _behavioral = behavioral;
        _store = store;
        _audit = audit;
        _config = config;
        _logger = logger;
    }

    // ── StartInterviewAsync ─────────────────────────────────

    public async Task<ApiResponseDto<InterviewSessionDto>> StartInterviewAsync(
        StartInterviewRequestDto request, Guid userId)
    {
        // Validar request
        var errors = ValidateStartRequest(request);
        if (errors.Count > 0)
            return ApiResponseDto<InterviewSessionDto>.Fail(errors);

        var tipo = request.TipoEntrevista.ToLowerInvariant();
        var nivel = NivelesValidos.First(n => n.Equals(request.Nivel, StringComparison.OrdinalIgnoreCase));

        // Recuperar contexto según tipo
        var contextoTecnico = new List<QdrantSearchResult>();
        var contextoBehavioral = new List<BehavioralQuestionContext>();

        var preguntasTecnicasCount = int.TryParse(_config["Interview:PreguntasTecnicasCount"], out var tc) ? tc : 10;
        var preguntasBehavioralCount = int.TryParse(_config["Interview:PreguntasBehavioralCount"], out var bc) ? bc : 5;

        if (tipo is "tecnica" or "mixta")
        {
            try
            {
                contextoTecnico = await _qdrant.SearchAsync(new QdrantSearchRequest
                {
                    Category = "Technical",
                    DifficultyLevel = nivel,
                    Limit = preguntasTecnicasCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo conectar a Qdrant, continuando sin contexto técnico");
            }
        }

        if (tipo is "behavioral" or "mixta")
        {
            try
            {
                contextoBehavioral = await _behavioral.GetRandomQuestionsAsync(
                    preguntasBehavioralCount, nivel);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo obtener preguntas behavioral, continuando sin contexto behavioral");
            }
        }

        // Construir system prompt
        var systemPrompt = BuildSystemPrompt(tipo, request.Tecnologia, nivel,
            contextoTecnico, contextoBehavioral);

        // Crear sesión
        var session = new InterviewSession
        {
            Id = Guid.NewGuid(),
            UsuarioId = userId,
            TipoEntrevista = tipo,
            Tecnologia = request.Tecnologia,
            Nivel = nivel,
            Estado = "EnCurso",
            SystemPrompt = systemPrompt,
            ContextoRecuperado = SerializeContext(contextoTecnico, contextoBehavioral),
            FechaInicio = DateTime.UtcNow,
            DuracionMinutos = Math.Min(request.DuracionMinutos,
                int.TryParse(_config["Interview:MaxDuracionMinutos"], out var max) ? max : 60)
        };

        await _store.CreateAsync(session);

        // Persistir system prompt como primer mensaje
        await _store.AddMessageAsync(session.Id, new InterviewMessage
        {
            Rol = "system",
            Contenido = systemPrompt,
            Orden = 0,
            FechaCreacion = DateTime.UtcNow
        });

        // Llamar a Ollama para obtener mensaje inicial del entrevistador
        string respuestaEntrevistador;
        try
        {
            respuestaEntrevistador = await _ollama.GenerateAsync(new OllamaRequest
            {
                Messages = new List<OllamaChatMessage>
                {
                    new() { Role = "system", Content = systemPrompt }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al comunicarse con Ollama");
            return ApiResponseDto<InterviewSessionDto>.Fail(
                "No se pudo conectar con el servicio de IA. Verifique que Ollama esté ejecutándose.");
        }

        // Persistir respuesta del entrevistador
        await _store.AddMessageAsync(session.Id, new InterviewMessage
        {
            Rol = "entrevistador",
            Contenido = respuestaEntrevistador,
            Orden = 1,
            FechaCreacion = DateTime.UtcNow
        });

        await _audit.LogAsync(userId, "INTERVIEW_START", "InterviewSessions",
            session.Id.ToString());

        return ApiResponseDto<InterviewSessionDto>.Ok(MapToSessionDto(session, new List<InterviewMessage>
        {
            new() { Rol = "entrevistador", Contenido = respuestaEntrevistador, FechaCreacion = DateTime.UtcNow }
        }), "Entrevista iniciada exitosamente");
    }

    // ── SendMessageAsync ────────────────────────────────────

    public async Task<ApiResponseDto<InterviewMessageDto>> SendMessageAsync(
        Guid sessionId, SendMessageRequestDto request, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(request.Mensaje))
            return ApiResponseDto<InterviewMessageDto>.Fail("El mensaje no puede estar vacío.");

        var session = await _store.GetByIdAsync(sessionId);
        if (session is null)
            return ApiResponseDto<InterviewMessageDto>.Fail("Sesión de entrevista no encontrada.");

        if (session.UsuarioId != userId)
            return ApiResponseDto<InterviewMessageDto>.Fail("No tiene acceso a esta sesión.");

        if (session.Estado != "EnCurso")
            return ApiResponseDto<InterviewMessageDto>.Fail("Esta entrevista ya no está en curso.");

        var maxMensajes = int.TryParse(_config["Interview:MaxMensajesPorSesion"], out var mm) ? mm : 50;
        var mensajes = await _store.GetMessagesAsync(sessionId);

        if (mensajes.Count >= maxMensajes)
            return ApiResponseDto<InterviewMessageDto>.Fail(
                "Se alcanzó el límite de mensajes para esta entrevista. Finalize la entrevista.");

        // Persistir mensaje del candidato
        var ordenCandidato = mensajes.Count;
        await _store.AddMessageAsync(sessionId, new InterviewMessage
        {
            Rol = "candidato",
            Contenido = request.Mensaje,
            Orden = ordenCandidato,
            FechaCreacion = DateTime.UtcNow
        });

        // Construir historial para Ollama
        var ollamaMessages = BuildOllamaMessages(session.SystemPrompt, mensajes, request.Mensaje);

        // Llamar a Ollama
        string respuesta;
        try
        {
            respuesta = await _ollama.GenerateAsync(new OllamaRequest { Messages = ollamaMessages });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al comunicarse con Ollama en sesión {SessionId}", sessionId);
            return ApiResponseDto<InterviewMessageDto>.Fail(
                "Error al comunicarse con el servicio de IA. Intente de nuevo.");
        }

        // Persistir respuesta del entrevistador
        var ordenEntrevistador = ordenCandidato + 1;
        await _store.AddMessageAsync(sessionId, new InterviewMessage
        {
            Rol = "entrevistador",
            Contenido = respuesta,
            Orden = ordenEntrevistador,
            FechaCreacion = DateTime.UtcNow
        });

        return ApiResponseDto<InterviewMessageDto>.Ok(new InterviewMessageDto
        {
            Rol = "entrevistador",
            Contenido = respuesta,
            Timestamp = DateTime.UtcNow
        }, "Respuesta del entrevistador recibida");
    }

    // ── EndInterviewAsync ───────────────────────────────────

    public async Task<ApiResponseDto<InterviewEvaluationDto>> EndInterviewAsync(
        Guid sessionId, Guid userId)
    {
        var session = await _store.GetByIdAsync(sessionId);
        if (session is null)
            return ApiResponseDto<InterviewEvaluationDto>.Fail("Sesión de entrevista no encontrada.");

        if (session.UsuarioId != userId)
            return ApiResponseDto<InterviewEvaluationDto>.Fail("No tiene acceso a esta sesión.");

        if (session.Estado == "Finalizada")
        {
            // Si ya fue evaluada, retornar la evaluación existente
            if (session.EvaluacionJson is not null)
            {
                var existing = JsonSerializer.Deserialize<InterviewEvaluationDto>(
                    session.EvaluacionJson, JsonOptions);
                if (existing is not null)
                    return ApiResponseDto<InterviewEvaluationDto>.Ok(existing, "Evaluación ya existente");
            }
        }

        if (session.Estado != "EnCurso")
            return ApiResponseDto<InterviewEvaluationDto>.Fail("Esta entrevista no está en curso.");

        var mensajes = await _store.GetMessagesAsync(sessionId);

        // Verificar que hubo conversación (al menos 1 respuesta del candidato)
        if (!mensajes.Any(m => m.Rol == "candidato"))
            return ApiResponseDto<InterviewEvaluationDto>.Fail(
                "No se puede evaluar una entrevista sin respuestas del candidato.");

        // Construir prompt de evaluación
        var promptEvaluacion = BuildEvaluationPrompt(session, mensajes);

        // Llamar a Ollama para evaluación (con retry para JSON)
        InterviewEvaluationDto? evaluacion = null;
        const int maxRetries = 2;

        for (int intento = 0; intento <= maxRetries; intento++)
        {
            string respuestaJson;
            try
            {
                respuestaJson = await _ollama.GenerateAsync(new OllamaRequest
                {
                    Messages = new List<OllamaChatMessage>
                    {
                        new() { Role = "system", Content = "Eres un evaluador experto. Responde EXCLUSIVAMENTE con JSON válido, sin markdown ni texto adicional." },
                        new() { Role = "user", Content = promptEvaluacion }
                    },
                    Options = new OllamaOptions { Temperature = 0.3f, MaxTokens = 2048 }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al comunicarse con Ollama para evaluación (intento {Intento})", intento + 1);
                if (intento == maxRetries)
                    return ApiResponseDto<InterviewEvaluationDto>.Fail(
                        "Error al comunicarse con el servicio de IA para la evaluación.");
                continue;
            }

            evaluacion = TryParseEvaluation(respuestaJson);
            if (evaluacion is not null) break;

            _logger.LogWarning("Intento {Intento}: Ollama no devolvió JSON válido para evaluación", intento + 1);
        }

        evaluacion ??= BuildFallbackEvaluation();

        // Actualizar sesión
        session.Estado = "Finalizada";
        session.FechaFin = DateTime.UtcNow;
        session.EvaluacionJson = JsonSerializer.Serialize(evaluacion, JsonOptions);
        await _store.UpdateAsync(session);

        await _audit.LogAsync(userId, "INTERVIEW_END", "InterviewSessions",
            session.Id.ToString(), null, session.EvaluacionJson);

        return ApiResponseDto<InterviewEvaluationDto>.Ok(evaluacion, "Entrevista evaluada exitosamente");
    }

    // ── GetSessionAsync ─────────────────────────────────────

    public async Task<ApiResponseDto<InterviewSessionDto>> GetSessionAsync(
        Guid sessionId, Guid userId)
    {
        var session = await _store.GetByIdAsync(sessionId);
        if (session is null)
            return ApiResponseDto<InterviewSessionDto>.Fail("Sesión de entrevista no encontrada.");

        if (session.UsuarioId != userId)
            return ApiResponseDto<InterviewSessionDto>.Fail("No tiene acceso a esta sesión.");

        var mensajes = session.Mensajes?
            .Where(m => m.Rol != "system")
            .OrderBy(m => m.Orden)
            .ToList() ?? new List<InterviewMessage>();

        return ApiResponseDto<InterviewSessionDto>.Ok(
            MapToSessionDto(session, mensajes), "Sesión obtenida exitosamente");
    }

    // ── GetUserSessionsAsync ────────────────────────────────

    public async Task<ApiResponseDto<PaginatedResult<InterviewSessionListDto>>> GetUserSessionsAsync(
        Guid userId, int page, int pageSize)
    {
        var (items, totalCount) = await _store.GetByUserPaginatedAsync(userId, page, pageSize);

        var result = new PaginatedResult<InterviewSessionListDto>
        {
            Items = items.Select(MapToListDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return ApiResponseDto<PaginatedResult<InterviewSessionListDto>>.Ok(result);
    }

    // ══════════════════════════════════════════════════════════
    //  MÉTODOS PRIVADOS
    // ══════════════════════════════════════════════════════════

    private static List<string> ValidateStartRequest(StartInterviewRequestDto request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.TipoEntrevista) ||
            !TiposValidos.Contains(request.TipoEntrevista))
            errors.Add("El tipo de entrevista debe ser 'tecnica', 'behavioral' o 'mixta'.");

        if (string.IsNullOrWhiteSpace(request.Nivel) ||
            !NivelesValidos.Contains(request.Nivel))
            errors.Add("El nivel debe ser 'Junior', 'Mid' o 'Senior'.");

        var tipo = request.TipoEntrevista?.ToLowerInvariant();
        if (tipo is "tecnica" or "mixta" && string.IsNullOrWhiteSpace(request.Tecnologia))
            errors.Add("La tecnología es requerida para entrevistas técnicas o mixtas.");

        if (request.DuracionMinutos < 5 || request.DuracionMinutos > 120)
            errors.Add("La duración debe estar entre 5 y 120 minutos.");

        return errors;
    }

    private static string BuildSystemPrompt(
        string tipo, string? tecnologia, string nivel,
        List<QdrantSearchResult> contextoTecnico,
        List<BehavioralQuestionContext> contextoBehavioral)
    {
        var descripcionNivel = nivel switch
        {
            "Junior" => "Valida fundamentos, sintaxis básica, conceptos core. Sé paciente y guía al candidato. Preguntas directas, no trampas.",
            "Mid" => "Asume conocimiento de fundamentos. Pregunta sobre diseño, patrones, trade-offs. Espera que justifique decisiones técnicas con experiencia real.",
            "Senior" => "Pregunta sobre arquitectura, escalabilidad, liderazgo técnico, mentoring. Presenta escenarios complejos con restricciones. Espera que cuestione supuestos y proponga alternativas.",
            _ => ""
        };

        var prompt = $@"Eres un entrevistador técnico senior en una empresa de tecnología. Conduces entrevistas en español de forma profesional, empática y conversacional.

INSTRUCCIONES:
- Siempre responde en español.
- Haz UNA pregunta a la vez. Nunca listes múltiples preguntas.
- Después de cada respuesta del candidato, da un breve feedback o transición natural antes de la siguiente pregunta.
- Si la respuesta es superficial, haz follow-ups para profundizar (""¿Podrías darme un ejemplo concreto?"", ""¿Cómo manejarías el caso donde...?"").
- Si la respuesta es incorrecta, no corrijas directamente — reformula o guía con pistas.
- Adapta la dificultad al nivel {nivel}: {descripcionNivel}
- Mantén un tono profesional pero cercano, como una conversación real.
- NO generes respuestas del candidato. Solo responde como entrevistador.

TIPO DE ENTREVISTA: {tipo}
TECNOLOGÍA: {tecnologia ?? "General"}
NIVEL: {nivel}";

        // Bloque de contexto técnico
        if (contextoTecnico.Count > 0)
        {
            prompt += "\n\nCONTEXTO TÉCNICO (usa estas preguntas como guía, no las leas textualmente):";
            prompt += "\n---";
            for (int i = 0; i < contextoTecnico.Count; i++)
            {
                var q = contextoTecnico[i];
                prompt += $"\n{i + 1}. ";
                if (!string.IsNullOrWhiteSpace(q.QuestionText))
                    prompt += $"[Pregunta] {q.QuestionText}\n";
                if (!string.IsNullOrWhiteSpace(q.Text) && q.Text != q.QuestionText)
                    prompt += $"   Contexto: {q.Text}\n";
            }
            prompt += "---";
            prompt += "\nReformula estas preguntas con tu propio estilo. Puedes combinar temas o crear variaciones basadas en las respuestas del candidato.";
        }

        // Bloque de contexto behavioral
        if (contextoBehavioral.Count > 0)
        {
            prompt += "\n\nPREGUNTAS BEHAVIORAL (método STAR — Situación, Tarea, Acción, Resultado):";
            prompt += "\n---";
            for (int i = 0; i < contextoBehavioral.Count; i++)
            {
                var q = contextoBehavioral[i];
                prompt += $"\n{i + 1}. \"{q.QuestionText}\"";
                prompt += $"\n   Competencia evaluada: {q.Competency}";
                if (q.EvaluationCriteria.Count > 0)
                    prompt += $"\n   Criterios de evaluación: {string.Join("; ", q.EvaluationCriteria)}";
                if (q.RedFlags.Count > 0)
                    prompt += $"\n   Red flags: {string.Join("; ", q.RedFlags)}";
                prompt += "\n";
            }
            prompt += "---";
            prompt += "\nPara preguntas behavioral, busca respuestas con estructura STAR. Si el candidato no da estructura, guíalo: \"¿Cuál fue la situación específica?\"";
        }

        prompt += "\n\nComienza presentándote brevemente y haciendo la primera pregunta.";

        return prompt;
    }

    private static string BuildEvaluationPrompt(InterviewSession session, List<InterviewMessage> mensajes)
    {
        var historial = string.Join("\n", mensajes
            .Where(m => m.Rol != "system")
            .OrderBy(m => m.Orden)
            .Select(m =>
            {
                var rol = m.Rol == "entrevistador" ? "Entrevistador" : "Candidato";
                return $"{rol}: {m.Contenido}";
            }));

        var competenciasBehavioral = session.TipoEntrevista is "behavioral" or "mixta"
            ? "\n- Competencias conductuales (liderazgo, trabajo en equipo, adaptabilidad)"
            : "";

        return $@"Analiza la siguiente entrevista técnica y genera una evaluación estructurada.

ENTREVISTA COMPLETA:
---
{historial}
---

TIPO: {session.TipoEntrevista} | TECNOLOGÍA: {session.Tecnologia ?? "General"} | NIVEL ESPERADO: {session.Nivel}

Responde EXCLUSIVAMENTE con un JSON válido (sin markdown, sin ```):
{{
  ""puntuacionGeneral"": <0-100>,
  ""nivelEvaluado"": ""<Junior|Mid|Senior>"",
  ""competencias"": [
    {{
      ""nombre"": ""<nombre de la competencia>"",
      ""puntuacion"": <1-5>,
      ""justificacion"": ""<evidencia concreta de la entrevista>""
    }}
  ],
  ""resumenGeneral"": ""<2-3 oraciones>"",
  ""fortalezas"": [""<fortaleza 1>"", ""<fortaleza 2>""],
  ""areasDeMejora"": [""<area 1>"", ""<area 2>""],
  ""recomendacion"": ""<Apto|Con reservas|No apto>""
}}

COMPETENCIAS A EVALUAR:
- Conocimiento técnico (dominio del tema)
- Resolución de problemas (razonamiento, enfoque analítico)
- Comunicación (claridad, estructura de respuestas)
- Experiencia práctica (ejemplos reales, profundidad){competenciasBehavioral}

Sé objetivo. Basa la puntuación en evidencia concreta de las respuestas, no en suposiciones.";
    }

    private static List<OllamaChatMessage> BuildOllamaMessages(
        string systemPrompt, List<InterviewMessage> historial, string mensajeActual)
    {
        var messages = new List<OllamaChatMessage>
        {
            new() { Role = "system", Content = systemPrompt }
        };

        foreach (var msg in historial.Where(m => m.Rol != "system").OrderBy(m => m.Orden))
        {
            var role = msg.Rol == "entrevistador" ? "assistant" : "user";
            messages.Add(new OllamaChatMessage { Role = role, Content = msg.Contenido });
        }

        // Agregar el mensaje actual del candidato
        messages.Add(new OllamaChatMessage { Role = "user", Content = mensajeActual });

        return messages;
    }

    private static InterviewEvaluationDto? TryParseEvaluation(string respuesta)
    {
        try
        {
            // Intentar extraer JSON si viene envuelto en markdown
            var json = respuesta.Trim();
            var startIdx = json.IndexOf('{');
            var endIdx = json.LastIndexOf('}');
            if (startIdx >= 0 && endIdx > startIdx)
                json = json[startIdx..(endIdx + 1)];

            return JsonSerializer.Deserialize<InterviewEvaluationDto>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    private static InterviewEvaluationDto BuildFallbackEvaluation()
    {
        return new InterviewEvaluationDto
        {
            PuntuacionGeneral = 0,
            NivelEvaluado = "N/A",
            Competencias = new List<CompetenciaEvaluadaDto>(),
            ResumenGeneral = "No se pudo generar la evaluación automática. Revise la entrevista manualmente.",
            Fortalezas = new List<string>(),
            AreasDeMejora = new List<string>(),
            Recomendacion = "Con reservas"
        };
    }

    private static string? SerializeContext(
        List<QdrantSearchResult> tecnico, List<BehavioralQuestionContext> behavioral)
    {
        if (tecnico.Count == 0 && behavioral.Count == 0) return null;

        return JsonSerializer.Serialize(new { tecnico, behavioral }, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    // ── Mappers ─────────────────────────────────────────────

    private static InterviewSessionDto MapToSessionDto(InterviewSession session, List<InterviewMessage> mensajes)
    {
        // Filtrar mensajes de sistema para el cliente
        var mensajesVisibles = mensajes.Where(m => m.Rol != "system").ToList();

        return new InterviewSessionDto
        {
            Id = session.Id,
            TipoEntrevista = session.TipoEntrevista,
            Tecnologia = session.Tecnologia,
            Nivel = session.Nivel,
            Estado = session.Estado,
            FechaInicio = session.FechaInicio,
            FechaFin = session.FechaFin,
            DuracionMinutos = session.DuracionMinutos,
            TotalMensajes = mensajesVisibles.Count,
            Mensajes = mensajesVisibles.Select(m => new InterviewMessageDto
            {
                Rol = m.Rol,
                Contenido = m.Contenido,
                Timestamp = m.FechaCreacion
            }).ToList()
        };
    }

    private static InterviewSessionListDto MapToListDto(InterviewSession session)
    {
        double? puntuacion = null;
        if (session.EvaluacionJson is not null)
        {
            try
            {
                var eval = JsonSerializer.Deserialize<InterviewEvaluationDto>(
                    session.EvaluacionJson, JsonOptions);
                puntuacion = eval?.PuntuacionGeneral;
            }
            catch { /* ignorar errores de parseo */ }
        }

        return new InterviewSessionListDto
        {
            Id = session.Id,
            TipoEntrevista = session.TipoEntrevista,
            Tecnologia = session.Tecnologia,
            Nivel = session.Nivel,
            Estado = session.Estado,
            FechaInicio = session.FechaInicio,
            FechaFin = session.FechaFin,
            TotalMensajes = session.Mensajes?.Count(m => m.Rol != "system") ?? 0,
            PuntuacionGeneral = puntuacion
        };
    }
}

using System.Text.Json;
using Interview_Base.Models.Interview;
using Interview_Base.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Interview_Base.Services;

public class OllamaClient : IOllamaClient
{
    private readonly HttpClient _http;
    private readonly ILogger<OllamaClient> _logger;
    private readonly string _model;
    private readonly float _temperature;
    private readonly int _maxTokens;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public OllamaClient(HttpClient http, IConfiguration config, ILogger<OllamaClient> logger)
    {
        _http = http;
        _logger = logger;
        _model = config["Ollama:Model"] ?? "llama3.1:8b";
        _temperature = float.TryParse(config["Ollama:Temperature"], out var temp) ? temp : 0.7f;
        _maxTokens = int.TryParse(config["Ollama:MaxTokens"], out var max) ? max : 1024;
    }

    public async Task<string> GenerateAsync(OllamaRequest request, CancellationToken ct = default)
    {
        // Aplicar configuración por defecto si no se especificó
        request.Model ??= _model;
        request.Options ??= new OllamaOptions();
        if (request.Options.Temperature == 0) request.Options.Temperature = _temperature;
        if (request.Options.MaxTokens == 0) request.Options.MaxTokens = _maxTokens;

        _logger.LogInformation("Enviando solicitud a Ollama (modelo: {Model}, mensajes: {Count})",
            request.Model, request.Messages.Count);

        var json = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _http.PostAsync("/api/chat", content, ct);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync(ct);
        var ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(responseBody, JsonOptions);

        if (ollamaResponse?.Message?.Content is null)
        {
            _logger.LogWarning("Ollama devolvió una respuesta vacía");
            return string.Empty;
        }

        _logger.LogInformation("Respuesta de Ollama recibida ({Tokens} tokens evaluados)",
            ollamaResponse.EvalCount);

        return ollamaResponse.Message.Content;
    }

    public async Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _http.GetAsync("/api/tags", ct);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ollama no está disponible en {BaseAddress}", _http.BaseAddress);
            return false;
        }
    }
}

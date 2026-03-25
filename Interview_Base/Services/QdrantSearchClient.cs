using System.Text.Json;
using Interview_Base.Models.Interview;
using Interview_Base.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Interview_Base.Services;

public class QdrantSearchClient : IQdrantSearchClient
{
    private readonly HttpClient _http;
    private readonly ILogger<QdrantSearchClient> _logger;
    private readonly string _collectionName;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public QdrantSearchClient(HttpClient http, IConfiguration config, ILogger<QdrantSearchClient> logger)
    {
        _http = http;
        _logger = logger;
        _collectionName = config["Qdrant:CollectionName"] ?? "interview_questions";
    }

    public async Task<List<QdrantSearchResult>> SearchAsync(QdrantSearchRequest request, CancellationToken ct = default)
    {
        var filters = new List<QdrantCondition>();

        if (!string.IsNullOrWhiteSpace(request.Category))
            filters.Add(new QdrantCondition { Key = "category", Match = new QdrantMatch { Value = request.Category } });

        if (!string.IsNullOrWhiteSpace(request.DifficultyLevel))
            filters.Add(new QdrantCondition { Key = "difficulty_level", Match = new QdrantMatch { Value = request.DifficultyLevel } });

        var scrollRequest = new QdrantScrollRequest
        {
            Filter = filters.Count > 0 ? new QdrantFilter { Must = filters } : null,
            Limit = request.Limit,
            WithPayload = true,
            WithVector = false
        };

        var url = $"/collections/{_collectionName}/points/scroll";

        _logger.LogInformation("Buscando en Qdrant: colección={Collection}, categoría={Category}, nivel={Level}, límite={Limit}",
            _collectionName, request.Category, request.DifficultyLevel, request.Limit);

        var json = JsonSerializer.Serialize(scrollRequest, JsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _http.PostAsync(url, content, ct);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync(ct);
        var scrollResponse = JsonSerializer.Deserialize<QdrantScrollResponse>(responseBody, JsonOptions);

        var points = scrollResponse?.Result?.Points;
        if (points is null || points.Count == 0)
        {
            _logger.LogInformation("Qdrant no devolvió resultados para los filtros aplicados");
            return new List<QdrantSearchResult>();
        }

        var results = new List<QdrantSearchResult>();

        foreach (var point in points)
        {
            if (point.Payload is null) continue;

            results.Add(new QdrantSearchResult
            {
                Text = GetPayloadString(point.Payload, "text"),
                QuestionText = GetPayloadString(point.Payload, "question_text"),
                Category = GetPayloadString(point.Payload, "category"),
                DifficultyLevel = GetPayloadString(point.Payload, "difficulty_level"),
                SourceName = GetPayloadString(point.Payload, "source_name")
            });
        }

        _logger.LogInformation("Qdrant devolvió {Count} resultados", results.Count);
        return results;
    }

    public async Task<bool> IsAvailableAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _http.GetAsync("/collections", ct);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Qdrant no está disponible en {BaseAddress}", _http.BaseAddress);
            return false;
        }
    }

    private static string GetPayloadString(Dictionary<string, object> payload, string key)
    {
        if (!payload.TryGetValue(key, out var value))
            return string.Empty;

        // System.Text.Json deserializa los valores como JsonElement
        if (value is JsonElement element)
            return element.ValueKind == JsonValueKind.String ? element.GetString() ?? string.Empty : element.ToString();

        return value?.ToString() ?? string.Empty;
    }
}

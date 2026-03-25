using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Interview_Base.Models.Interview;

// Modelos para la API REST de Qdrant (scroll con filtros)

public class QdrantSearchRequest
{
    public string? Category { get; set; }
    public string? DifficultyLevel { get; set; }
    public int Limit { get; set; } = 15;
}

public class QdrantSearchResult
{
    public string Text { get; set; } = string.Empty;
    public string? QuestionText { get; set; }
    public string Category { get; set; } = string.Empty;
    public string DifficultyLevel { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
}

// Modelos de serialización para la API REST de Qdrant

public class QdrantScrollRequest
{
    [JsonPropertyName("filter")]
    public QdrantFilter? Filter { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 15;

    [JsonPropertyName("with_payload")]
    public bool WithPayload { get; set; } = true;

    [JsonPropertyName("with_vector")]
    public bool WithVector { get; set; } = false;
}

public class QdrantFilter
{
    [JsonPropertyName("must")]
    public List<QdrantCondition>? Must { get; set; }
}

public class QdrantCondition
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = null!;

    [JsonPropertyName("match")]
    public QdrantMatch Match { get; set; } = null!;
}

public class QdrantMatch
{
    [JsonPropertyName("value")]
    public object Value { get; set; } = null!;
}

public class QdrantScrollResponse
{
    [JsonPropertyName("result")]
    public QdrantScrollResult? Result { get; set; }
}

public class QdrantScrollResult
{
    [JsonPropertyName("points")]
    public List<QdrantPoint>? Points { get; set; }

    [JsonPropertyName("next_page_offset")]
    public object? NextPageOffset { get; set; }
}

public class QdrantPoint
{
    [JsonPropertyName("id")]
    public object Id { get; set; } = null!;

    [JsonPropertyName("payload")]
    public Dictionary<string, object>? Payload { get; set; }
}

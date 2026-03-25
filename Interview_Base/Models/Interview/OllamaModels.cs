using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Interview_Base.Models.Interview;

public class OllamaRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "llama3.1:8b";

    [JsonPropertyName("messages")]
    public List<OllamaChatMessage> Messages { get; set; } = new();

    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;

    [JsonPropertyName("options")]
    public OllamaOptions? Options { get; set; }
}

public class OllamaChatMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = null!; // "system", "user", "assistant"

    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;
}

public class OllamaOptions
{
    [JsonPropertyName("temperature")]
    public float Temperature { get; set; } = 0.7f;

    [JsonPropertyName("num_predict")]
    public int MaxTokens { get; set; } = 1024;
}

public class OllamaResponse
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public OllamaChatMessage? Message { get; set; }

    [JsonPropertyName("done")]
    public bool Done { get; set; }

    [JsonPropertyName("total_duration")]
    public long? TotalDuration { get; set; }

    [JsonPropertyName("eval_count")]
    public int? EvalCount { get; set; }
}

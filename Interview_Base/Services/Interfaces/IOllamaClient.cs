using Interview_Base.Models.Interview;

namespace Interview_Base.Services.Interfaces;

public interface IOllamaClient
{
    Task<string> GenerateAsync(OllamaRequest request, CancellationToken ct = default);
    Task<bool> IsAvailableAsync(CancellationToken ct = default);
}

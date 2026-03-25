using Interview_Base.Models.Interview;

namespace Interview_Base.Services.Interfaces;

public interface IQdrantSearchClient
{
    Task<List<QdrantSearchResult>> SearchAsync(QdrantSearchRequest request, CancellationToken ct = default);
    Task<bool> IsAvailableAsync(CancellationToken ct = default);
}

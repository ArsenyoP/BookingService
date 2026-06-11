using Booking.Application.Interfaces.Services;
using Microsoft.Extensions.AI;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using static Booking.Application.Interfaces.Services.IQdrantService;

namespace Booking.Infrastructure.Services
{
    public class QdrantService(IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator,
        IQdrantClient _qdrantClient) : IQdrantService
    {
        public async Task<IReadOnlyList<VectorSearchMatch>> SearchAsync(string userMessage,
            string city, decimal? maxPrice, int limit, string CollectionName, CancellationToken ct)
        {
            var embeddingResult = await _embeddingGenerator.GenerateAsync(userMessage, cancellationToken: ct);
            var queryVector = embeddingResult.Vector.ToArray();

            var filter = new Filter();

            filter.Must.Add(new Condition
            {
                Field = new FieldCondition
                {
                    Key = "city",
                    Match = new Match { Keyword = city.ToLower().Trim() }
                }
            });

            if (maxPrice.HasValue)
            {
                filter.Must.Add(new Condition
                {
                    Field = new FieldCondition
                    {
                        Key = "price",
                        Range = new Qdrant.Client.Grpc.Range { Lte = (double)maxPrice.Value }
                    }
                });
            }
            ;

            var searchResults = await _qdrantClient.SearchAsync(
                collectionName: CollectionName,
                vector: queryVector,
                filter: filter,
                limit: (ulong)limit,
                cancellationToken: ct
            );

            return searchResults
                .Select(hit => new VectorSearchMatch(
                    Id: Guid.Parse(hit.Id.Uuid),
                    Score: hit.Score
                ))
                .ToList();
        }
    }
}

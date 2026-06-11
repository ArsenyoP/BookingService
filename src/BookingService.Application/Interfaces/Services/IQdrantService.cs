namespace Booking.Application.Interfaces.Services
{
    public interface IQdrantService
    {
        public record VectorSearchMatch(Guid Id, double Score);

        Task<IReadOnlyList<VectorSearchMatch>> SearchAsync(
                string userMessage,
                string city,
                decimal? maxPrice,
                int limit,
                string CollectionName,
                CancellationToken ct);
    }
}

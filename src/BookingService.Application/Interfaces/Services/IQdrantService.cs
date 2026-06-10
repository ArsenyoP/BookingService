namespace Booking.Application.Interfaces.Services
{
    public interface IQdrantService
    {
        public record VectorSearchMatch(Guid RoomId, double Score);

        Task<IReadOnlyList<VectorSearchMatch>> SearchAsync(
                string userMessage,
                string city,
                decimal? maxPrice,
                int limit,
                CancellationToken ct);
    }
}

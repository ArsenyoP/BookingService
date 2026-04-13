using Booking.Application.DTOs.Listings;
using Booking.Domain.Entities;


namespace Booking.Application.Interfaces.IQueries
{
    public interface IListingQueries
    {
        public Task<Listing?> GetEntityByIdAsync(Guid id, CancellationToken ct = default);
        public Task<ListingResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        public Task<IReadOnlyList<ListingResponseDto>?> GetAllPagedAsync(int page, int pageSize, CancellationToken ct = default);
    }
}

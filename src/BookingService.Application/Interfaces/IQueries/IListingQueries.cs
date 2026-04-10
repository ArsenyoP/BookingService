using Booking.Application.DTOs.Listings;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Interfaces.IQueries
{
    public interface IListingQueries
    {
        public Task<Listing> GetEntityByIdAsync(Guid id, CancellationToken ct = default);
        public Task<IReadOnlyList<ListingResponseDto>?> GetAllPaged(int page, int pageSize, CancellationToken ct = default);
    }
}

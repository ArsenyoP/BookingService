using Booking.Application.DTOs.Listings;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Queries
{
    public class ListingQueries(string connectionString) : IListingQueries
    {
        public Task<IReadOnlyList<ListingResponseDto>?> GetAllPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Listing?> GetEntityByIdAsync(Guid id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}

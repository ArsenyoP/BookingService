using System;
using System.Collections.Generic;
using System.Linq;
using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Repositories
{
    public class ListingRepository(AppDbContext _dbContext) : IListingRepository
    {
        public void Add(Listing listing)
        {
            ArgumentNullException.ThrowIfNull(listing);
            _dbContext.Listings.Add(listing);
        }

        public void Delete(Listing obj)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<Listing>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            return await _dbContext.Listings
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
        }

        public async Task<Listing?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.Listings.FirstOrDefaultAsync(l => l.Id == id, ct);
        }
    }
}

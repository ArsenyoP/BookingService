using Booking.Domain.Entities;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Repositories
{
    public class ListingRepository(AppDbContext _dbContext) : IListingRepository
    {
        public void Add(Listing obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(Listing obj)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Listing?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbContext.Listings.FirstOrDefaultAsync(l => l.Id == id);
        }
    }
}

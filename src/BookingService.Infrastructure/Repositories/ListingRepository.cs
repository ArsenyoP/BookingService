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
    }
}

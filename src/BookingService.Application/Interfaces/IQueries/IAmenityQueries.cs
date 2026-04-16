using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Interfaces.IQueries
{
    public interface IAmenityQueries
    {
        public Task<Amenity?> GetByNameAsync(string name, CancellationToken ct = default);
    }
}

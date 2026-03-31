using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Interfaces.IRepositories
{
    public interface IListingRepository : IBaseRepository<Listing>
    {
        //Override GetById to include amenities
    }
}

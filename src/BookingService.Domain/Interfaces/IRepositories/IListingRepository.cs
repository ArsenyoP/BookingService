using Booking.Domain.Entities;

namespace Booking.Domain.Interfaces.IRepositories
{
    public interface IListingRepository : IBaseRepository<Listing>
    {
        //TODO: Override GetById to include amenities
    }
}

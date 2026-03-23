using Booking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Domain.Enums;
using Booking.Domain.Errors;

namespace Booking.Domain.Entities
{
    public class Room : Entity
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        public RoomType Type { get; private set; }
        public decimal PricePerNight { get; private set; }

        public int AdultsCapacity { get; private set; }
        public int ChildrenCapacity { get; private set; }


        private readonly List<Amenity> _amenity = new();
        public IReadOnlyCollection<Amenity> Amenities => _amenity.ToList();

        public Result AddAmentity(Amenity amenity)
        {
            if (_amenity.Any(x => x.Id == amenity.Id))
            {
                return Result.Failure(RoomErrors.AmenityAlreadyExists);
            }

            _amenity.Add(amenity);
            return Result.Success();
        }
    }
}

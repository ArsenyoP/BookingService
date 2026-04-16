using Booking.Domain.Common;
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

        public Guid ListingId { get; private set; }
        private Listing _listing { get; set; }

        private readonly List<Amenity> _amenity = new();
        public IReadOnlyCollection<Amenity> Amenities => _amenity.AsReadOnly();


        private Room() { }

        private Room(
            string title,
            string description,
            RoomType type,
            decimal pricePerNight,
            int adultsCapacity,
            int childrenCapacity, Guid listingId)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Type = type;
            PricePerNight = pricePerNight;
            AdultsCapacity = adultsCapacity;
            ChildrenCapacity = childrenCapacity;
            ListingId = listingId;
        }

        //TODO: Check whether listing exists in application layer
        public static Result<Room> Create(
            string title,
            string description,
            RoomType type,
            decimal pricePerNight,
            int adultsCapacity,
            int childrenCapacity, Guid listingId)
        {
            if (string.IsNullOrWhiteSpace(title))
                return Result<Room>.Failure(RoomErrors.EmptyTitle);


            if (pricePerNight <= 0)
                return Result<Room>.Failure(RoomErrors.NegativePrice);

            if (adultsCapacity + childrenCapacity <= 0)
                return Result<Room>.Failure(RoomErrors.NegativeNumberCapacity);

            return Result<Room>.Success(new Room(title, description, type, pricePerNight, adultsCapacity, childrenCapacity, listingId));
        }

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

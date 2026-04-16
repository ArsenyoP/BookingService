using Booking.Domain.Common;
using Booking.Domain.Enums;
using Booking.Domain.Errors;
using Booking.Domain.ValueObjects;

namespace Booking.Domain.Entities
{
    public class Listing : Entity
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public Address Address { get; private set; }
        public ListingType ListingType { get; private set; }


        private readonly List<Amenity> _amenity = new();
        public IReadOnlyCollection<Amenity> Amenities => _amenity.AsReadOnly();



        private Listing() { }

        private Listing(string title, string description, Address address, ListingType listingType)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Address = address;
            ListingType = listingType;
        }

        public static Result<Listing> Create(string title, string description, Address address, ListingType listingType)
        {
            if (string.IsNullOrWhiteSpace(title))
                return Result<Listing>.Failure(ListingErrors.EmptyTitle);

            if (string.IsNullOrWhiteSpace(description))
                return Result<Listing>.Failure(ListingErrors.EmptyDescription);

            if (address is null)
                return Result<Listing>.Failure(ListingErrors.AddressRequired);

            return Result<Listing>.Success(new Listing(title, description, address, listingType));
        }

        public Result AddAmenity(Amenity amenity)
        {
            if (_amenity.Any(x => x.Id == amenity.Id))
            {
                return Result.Failure(ListingErrors.AmenityAlreadyAdded);
            }

            _amenity.Add(amenity);
            return Result.Success();
        }
    }
}

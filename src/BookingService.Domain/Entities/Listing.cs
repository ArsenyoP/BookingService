using Booking.Domain.Common;
using Booking.Domain.Errors;
using Booking.Domain.ValueObjects;

namespace Booking.Domain.Entities
{
    public class Listing : Entity
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public Address Address { get; private set; }
        public decimal PricePerNight { get; private set; }


        private readonly List<Amenity> _commonAmenities = new();
        public IReadOnlyCollection<Amenity> Amenities => _commonAmenities.AsReadOnly();


        private Listing() { }

        private Listing(string title, string description, Address address, decimal pricePerNight)
        {
            Title = title;
            Description = description;
            Address = address;
            PricePerNight = pricePerNight;
        }

        public static Result<Listing> Create(string title, string description, Address address, decimal pricePerNight)
        {
            if (string.IsNullOrWhiteSpace(title))
                return Result<Listing>.Failure(ListingErrors.EmptyTitle);

            if (string.IsNullOrWhiteSpace(description))
                return Result<Listing>.Failure(ListingErrors.EmptyDescription);

            if (pricePerNight <= 0)
                return Result<Listing>.Failure(ListingErrors.InvalidPricePerNight);

            if (address is null)
                return Result<Listing>.Failure(ListingErrors.AddressRequired);

            return Result<Listing>.Success(new Listing(title, description, address, pricePerNight));
        }

        public Result AddAmenity(Amenity amenity)
        {
            if (_commonAmenities.Any(x => x.Id == amenity.Id))
            {
                return Result.Failure(ListingErrors.AmenityAlreadyAdded);
            }

            _commonAmenities.Add(amenity);
            return Result.Success();
        }
    }
}

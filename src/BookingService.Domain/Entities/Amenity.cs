using Booking.Domain.Common;
using Booking.Domain.Enums;
using Booking.Domain.Errors;

namespace Booking.Domain.Entities
{
    public class Amenity : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public AmenityCategory Category { get; private set; }

        private Amenity() { }

        private Amenity(string name, AmenityCategory category)
        {
            Id = Guid.NewGuid();
            Name = name;
            Category = category;
        }

        public static Result<Amenity> Create(string name, AmenityCategory category)
        {
            if (string.IsNullOrWhiteSpace(name)) return Result<Amenity>.Failure(AmenityErrors.EmptyName);

            var amenity = new Amenity(name, category);

            return Result<Amenity>.Success(amenity);
        }
    }
}

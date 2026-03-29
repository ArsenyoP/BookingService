using Booking.Domain.Common;
using System.Collections.Generic;
using Booking.Domain.Errors;

namespace Booking.Domain.ValueObjects
{
    public record Address
    {
        public string Country { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string Street { get; init; } = string.Empty;
        public string HouseNumber { get; init; } = string.Empty;
        public int Floor { get; init; }

        private Address() { }

        private Address(string country, string city, string street, string houseNumber, int floor)
        {
            Country = country;
            City = city;
            Street = street;
            HouseNumber = houseNumber;
            Floor = floor;
        }

        public static Result<Address> Create(string country, string city, string street, string houseNumber, int floor)
        {
            string[] stringsToValidate = { country, city, street, houseNumber };

            if (stringsToValidate.Any(string.IsNullOrWhiteSpace))
            {
                return Result<Address>.Failure(AddressErrors.InvalidAddress);
            }

            return Result<Address>.Success(new Address(country, city, street, houseNumber, floor));
        }
    }
}

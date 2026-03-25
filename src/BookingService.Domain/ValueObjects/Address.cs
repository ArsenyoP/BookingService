using Booking.Domain.Common;
using System.Collections.Generic;
using Booking.Domain.Errors;

namespace Booking.Domain.ValueObjects
{
    //public sealed class Address : ValueObject
    //{
    //    public string Country { get; }
    //    public string City { get; }
    //    public string Street { get; }
    //    public string HouseNumber { get; }
    //    public int Floor { get; }

    //    public Address(string country, string city, string street, string houseNumber, int floor)
    //    {
    //        Country = country;
    //        City = city;
    //        Street = street;
    //        HouseNumber = houseNumber;
    //        Floor = floor;
    //    }

    //    protected override IEnumerable<object?> GetEqualityComponents()
    //    {
    //        yield return Country;
    //        yield return City;
    //        yield return Street;
    //        yield return HouseNumber;
    //        yield return Floor;
    //    }
    //}

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

        public Result<Address> Create(string country, string city, string street, string houseNumber, int floor)
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

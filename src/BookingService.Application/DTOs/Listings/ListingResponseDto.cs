using Booking.Domain.Enums;

namespace Booking.Application.DTOs.Listings
{
    public record ListingResponseDto(
        Guid Id,
        string Title,
        string Description,
        string Country,
        string City,
        string Street,
        string HouseNumber,
        int Floor,
        ListingType ListingType);
}

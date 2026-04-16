using Booking.Domain.Enums;

namespace Booking.Application.DTOs.Amenities
{
    public record AmenityDto(string Name, AmenityCategory Category);
}

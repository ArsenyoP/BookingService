using Booking.Application.Abstractions;
using Booking.Application.DTOs.Amenities;

namespace Booking.Application.UseCases.Amenities.GetAllAmenities
{
    public sealed record GetAllAmenitiesQuery(int Page, int PageSize) : IQuery<IReadOnlyList<AmenityDto>>;
}

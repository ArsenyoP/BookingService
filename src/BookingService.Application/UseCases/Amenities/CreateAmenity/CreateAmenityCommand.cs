using Booking.Application.Abstractions;
using Booking.Domain.Enums;

namespace Booking.Application.UseCases.Amenities
{
    public sealed record CreateAmenityCommand(string Name, AmenityCategory Category) : ICommand<Guid>;

}

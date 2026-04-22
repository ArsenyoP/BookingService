using Booking.Application.Abstractions;

namespace Booking.Application.UseCases.Amenities.DeleteAmenity
{
    public sealed record DeleteAmenityCommand(string Name) : ICommand<string>;

}

using Booking.Application.Abstractions;
using Booking.Domain.Enums;

namespace Booking.Application.UseCases.Room.CreateRoom;

public sealed record CreateRoomCommand(
    string Title,
    string Description,
    RoomType Type,
    decimal PricePerNight,
    int AdultsCapacity,
    int ChildrenCapacity,
    Guid ListingId) : ICommand<Guid>;


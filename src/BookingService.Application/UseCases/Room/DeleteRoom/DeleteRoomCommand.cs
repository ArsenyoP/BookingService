using Booking.Application.Abstractions;

namespace Booking.Application.UseCases.Room.DeleteRoom
{
    public sealed record DeleteRoomCommand(Guid roomId) : ICommand<Guid>, ICacheInvalidationCommand
    {
        public string Key => $"room:{roomId}";
    }
}

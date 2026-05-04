namespace Booking.Application.Abstractions
{
    public interface ICacheInvalidationCommand
    {
        string Key { get; }
    }
}

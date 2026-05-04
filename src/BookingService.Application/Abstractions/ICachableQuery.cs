namespace Booking.Application.Abstractions
{
    public interface ICachableQuery
    {
        string Key { get; }
        TimeSpan Expiration { get; }
    }
}

namespace Booking.Application.Helpers.Room
{
    public sealed record ListingQueryObject(
        string Title = "",
        string Country = "",
        string City = "",
        string Type = "",
        List<string>? AmenityNames = null,
        int PageSize = 10,
        int Page = 1);
}

namespace Booking.Application.Helpers.Room
{
    public sealed record RoomQueryObject(
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        int? MinAdults = null,
        int? MinChildren = null,
        string Type = "",
        List<string>? AmenityNames = null,
        int Page = 1,
        int PageSize = 10
        );
}

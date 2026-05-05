namespace Booking.Infrastructure.Seeding
{
    public sealed record ListingSeedingModel(
        string Title,
        string Description,
        string Country,
        string City,
        string Street,
        string HouseNumber,
        int Floor,
        string ListingType);
}

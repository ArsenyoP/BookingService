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

    public sealed record RoomSeedingModel(
        string ListingTitle,
        string Title,
        string Description,
        string Type,
        decimal PricePerNight,
        int AdultsCapacity,
        int ChildrenCapacity
        );

    public sealed record UserSeedModel(
        string FirstName,
        string LastName,
        string UserName,
        string Email,
        string Password,
        DateOnly DateOfBirth,
        string Role);
}

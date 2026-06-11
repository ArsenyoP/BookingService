namespace Booking.Application.DTOs.Chat
{
    public class SearchMatchDtos
    {
        public record RoomSearchMatchDto(Guid Id, string Title, decimal Price, decimal averageRating, double Confidence);
        public record ListingSearchMatchDto(Guid Id, string Title, decimal averageRating, double Confidence);

    }
}

using static Booking.Application.DTOs.Chat.SearchMatchDtos;

namespace Booking.Application.DTOs.Chat
{
    public sealed record ChatListingResponse(string aiResponse, List<ListingSearchMatchDto> listings);
}

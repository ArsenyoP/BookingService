using Booking.Application.UseCases.Chat.GetByText;

namespace Booking.Application.DTOs.Chat
{
    public sealed record ChatRoomResponse(string aiResponseText, List<RoomSearchMatchDto> rooms);
}

using Booking.Application.Abstractions;
using Booking.Application.DTOs.Chat;

namespace Booking.Application.UseCases.Chat.GetByText
{
    public sealed record GetByTextQuery(string city, string searchText, decimal? maxPrice = null)
        : IQuery<ChatRoomResponse>;
}

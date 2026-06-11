using Booking.Application.Abstractions;
using Booking.Application.DTOs.Chat;

namespace Booking.Application.UseCases.Chat.GetListingsByText
{
    public sealed record GetListingsByTextQuery : IQuery<ChatListingResponse>;
}

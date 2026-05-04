using Booking.Application.Abstractions;
using Booking.Application.DTOs.Bookings;


namespace Booking.Application.UseCases.Bookings.GetById
{
    public sealed record GetByIdQuery(Guid id) : IQuery<BookingResponseDto>, ICachableQuery
    {
        public string Key => $"booking:{id}";

        public TimeSpan Expiration => TimeSpan.FromMinutes(5);
    }
}

using System.Globalization;
using Booking.Application.Interfaces.IQueries;
using Booking.Application.Interfaces.Services;
using Booking.Domain.DomainEvents;
using Booking.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Booking.Application.UseCases.Bookings.CreateBooking
{
    public sealed class BookingCreatedEventHandler(
        IBookingQueries bookingQueries,
        IEmailService emailService,
        ILogger<BookingCreatedEventHandler> logger) : INotificationHandler<BookingCreatedDomainEvent>
    {
        public async Task Handle(BookingCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var emailData = await bookingQueries.GetConfirmationEmailDataAsync(
                notification.BookingId,
                cancellationToken);

            if (emailData is null)
            {
                logger.LogError(
                    "Booking {BookingId} not found when sending confirmation email",
                    notification.BookingId);
                throw new InvalidOperationException(BookingErrors.NotFound.Code);
            }

            if (string.IsNullOrWhiteSpace(emailData.GuestEmail))
            {
                logger.LogError(
                    "Guest email is missing for booking {BookingId}",
                    notification.BookingId);
                throw new InvalidOperationException("Guest email is required to send booking confirmation.");
            }

            string period = FormatPeriod(emailData.StartDate, emailData.EndDate);
            string totalPrice = emailData.TotalPrice.ToString("C", CultureInfo.CurrentCulture);

            await emailService.SendBookingConfirmationAsync(
                emailData.GuestEmail,
                emailData.GuestName,
                emailData.RoomTitle,
                period,
                totalPrice,
                cancellationToken);

            logger.LogInformation(
                "Booking confirmation email sent for booking {BookingId} to {Email}",
                notification.BookingId,
                emailData.GuestEmail);
        }

        private static string FormatPeriod(DateTime startDate, DateTime endDate)
        {
            var start = DateOnly.FromDateTime(startDate);
            var end = DateOnly.FromDateTime(endDate);
            return $"{start:dd MMM yyyy} – {end:dd MMM yyyy}";
        }
    }
}

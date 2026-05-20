namespace Booking.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default);

        Task SendBookingConfirmationAsync(
            string toEmail,
            string guestName,
            string roomTitle,
            string period,
            string totalPrice,
            CancellationToken ct = default);
    }
}

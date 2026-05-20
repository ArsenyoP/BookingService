using Booking.Application.Interfaces.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Text;
using Web.API.Models.Settings;

namespace Booking.Infrastructure.Services
{
    internal sealed class EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger) : IEmailService
    {
        private const string TemplatesFolder = "EmailTemplates";
        private const string BookingConfirmationFileName = "BookingConfirmation.html";


        public async Task SendBookingConfirmationAsync(
            string toEmail,
            string guestName,
            string roomTitle,
            string period,
            string totalPrice,
            CancellationToken ct = default)
        {
            string body = await BuildBookingConfirmationBodyAsync(
                guestName,
                roomTitle,
                period,
                totalPrice,
                ct);

            await SendAsync(toEmail, "Your booking is confirmed", body, ct);
        }

        public async Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default)
        {
            MimeMessage message = new();
            message.From.Add(new MailboxAddress(settings.Value.SenderName, settings.Value.SenderEmail));
            message.To.Add(new MailboxAddress(string.Empty, toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using SmtpClient client = new();
            await client.ConnectAsync(
                settings.Value.Host,
                settings.Value.Port,
                MailKit.Security.SecureSocketOptions.StartTls,
                ct);
            await client.AuthenticateAsync(settings.Value.SenderEmail, settings.Value.Password, ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            logger.LogInformation("Email sent to {Email}", toEmail);
        }

        public async Task<string> BuildBookingConfirmationBodyAsync(
            string guestName,
            string roomTitle,
            string period,
            string totalPrice,
            CancellationToken ct = default)
        {
            IReadOnlyDictionary<string, string> placeholders = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["GuestName"] = guestName,
                ["RoomTitle"] = roomTitle,
                ["Period"] = period,
                ["TotalPrice"] = totalPrice,
            };

            return await LoadAndRenderTemplateAsync(BookingConfirmationFileName, placeholders, ct);
        }

        private static async Task<string> LoadAndRenderTemplateAsync(
            string templateFileName,
            IReadOnlyDictionary<string, string> placeholders,
            CancellationToken ct)
        {
            string templatePath = Path.Combine(AppContext.BaseDirectory, TemplatesFolder, templateFileName);

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException(
                    $"Email template '{templateFileName}' was not found at '{templatePath}'. " +
                    "Ensure the template is included as Content with CopyToOutputDirectory=PreserveNewest.",
                    templatePath);
            }

            string template = await File.ReadAllTextAsync(templatePath, ct);
            return ReplacePlaceholders(template, placeholders);
        }

        private static string ReplacePlaceholders(string template, IReadOnlyDictionary<string, string> placeholders)
        {
            StringBuilder result = new(template);

            foreach (KeyValuePair<string, string> entry in placeholders)
            {
                string token = $"{{{{{entry.Key}}}}}";
                result.Replace(token, entry.Value ?? string.Empty);
            }

            return result.ToString();
        }
    }
}

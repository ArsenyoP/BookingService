namespace Booking.Application.Interfaces.Services
{
    public interface IChatService
    {
        Task<string> GenerateRoomAiResponse(string userContext, string systemMessage, CancellationToken ct = default);
    }
}

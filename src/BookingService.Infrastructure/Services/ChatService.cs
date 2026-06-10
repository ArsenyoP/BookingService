using Booking.Application.Interfaces.Services;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Booking.Infrastructure.Services
{
    public class ChatService(IChatCompletionService _chatCompletionService) : IChatService
    {
        public async Task<string> GenerateRoomAiResponse(string userContext, string systemMessage, CancellationToken ct = default)
        {
            var chatHistory = new ChatHistory();

            chatHistory.AddSystemMessage(systemMessage);
            chatHistory.AddUserMessage(userContext);

            var response = await _chatCompletionService.GetChatMessageContentAsync(
                chatHistory,
                cancellationToken: ct);

            return response.Content ?? "Не вдалося згенерувати відповідь.";
        }
    }
}

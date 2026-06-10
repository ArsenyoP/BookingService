using Booking.Application.UseCases.Chat.GetByText;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Endpoints
{
    public static class ChatEndpoints
    {
        public static void MapChatEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/chat")
                           .RequireRateLimiting("fixed")
                           .WithTags("Chat");

            group.MapPost("/", GetByText);
        }

        private static async Task<IResult> GetByText(
            [FromBody] GetByTextQuery query,
            ISender _sender,
            CancellationToken ct)
        {
            var result = await _sender.Send(query, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

    }
}

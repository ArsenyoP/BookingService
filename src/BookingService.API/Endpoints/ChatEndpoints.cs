using Booking.Application.UseCases.Chat.GetByText;
using Booking.Application.UseCases.Chat.GetListingsByText;
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

            group.MapPost("/room", GetRoomByText);
            group.MapPost("/lisitng", GetListingByText);
        }

        private static async Task<IResult> GetRoomByText(
            [FromBody] GetByTextQuery query,
            ISender _sender,
            CancellationToken ct)
        {
            var result = await _sender.Send(query, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> GetListingByText(
            [FromBody] GetListingsByTextQuery query,
            ISender _sender,
            CancellationToken ct)
        {
            var result = await _sender.Send(query, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

    }
}

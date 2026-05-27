using Booking.Application.Helpers.Room;
using Booking.Application.UseCases.Room.CreateRoom;
using Booking.Application.UseCases.Room.DeleteRoom;
using Booking.Application.UseCases.Room.GetAllRooms;
using Booking.Application.UseCases.Room.GetById;
using Booking.Application.UseCases.Room.GetByListingId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Endpoints
{
    public static class RoomEndpoints
    {
        public static void MapRoomEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/room")
                           .RequireRateLimiting("fixed")
                           .WithTags("Rooms");

            group.MapGet("/", GetAll);
            group.MapGet("/{id:guid}", GetById);
            group.MapGet("/listingId/{id:guid}", GetByListingId);

            group.MapPost("/", CreateRoom)
                 .RequireRateLimiting("write-limiter");

            group.MapDelete("/{id:guid}", DeleteRoom);
        }

        // --- Методи-обробники (Handlers) ---

        private static async Task<IResult> GetAll(
            [FromQuery] RoomQueryObject queryObject,
            ISender _sender,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetAllRoomsQuery(queryObject), ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> GetById(
            Guid id,
            ISender _sender,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetByIdQuery(id), ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> GetByListingId(
            Guid id,
            ISender _sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetByListingIdQuery(page, pageSize, id), ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> CreateRoom(
            [FromBody] CreateRoomCommand command,
            ISender _sender,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Created($"/api/bookings/{result.Value}", result.Value)
                : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> DeleteRoom(
            Guid id,
            ISender _sender,
            CancellationToken ct = default)
        {
            var command = new DeleteRoomCommand(id);
            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }
    }
}
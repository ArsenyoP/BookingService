using Booking.Application.UseCases.Amenities;
using Booking.Application.UseCases.Amenities.AddAmenityToListing;
using Booking.Application.UseCases.Amenities.AddAmenityToRoom;
using Booking.Application.UseCases.Amenities.DeleteAmenity;
using Booking.Application.UseCases.Amenities.GetAllAmenities;
using Booking.Application.UseCases.Amenities.RemoveAmenityFromListing;
using Booking.Application.UseCases.Amenities.RemoveAmenityFromRoom;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Endpoints
{
    public static class AmenityEndpoints
    {
        public static void MapAmenityEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/amenities")
                           .RequireRateLimiting("fixed")
                           .WithTags("Amenities");

            group.MapGet("/", GetAll);
            group.MapPost("/", CreateAmenity);
            group.MapPost("/addToRoom", AddAmenityToRoom);
            group.MapPost("/removeFromRoom", RemoveAmenityFromRoom).RequireAuthorization();
            group.MapPost("/addToListing", AddAmenityToListing);
            group.MapPost("/RemoveFromListing", RemoveAmenityFromListing).RequireAuthorization();
            group.MapDelete("/{name}", Delete);
        }

        private static async Task<IResult> GetAll(
            ISender _sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var command = new GetAllAmenitiesQuery(page, pageSize);
            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> CreateAmenity(
            [FromBody] CreateAmenityCommand command,
            ISender _sender,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Created($"/api/amenities/{result.Value}", result.Value)
                : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> AddAmenityToRoom(
            [FromBody] AddAmenityToRoomCommand command,
            ISender _sender,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> RemoveAmenityFromRoom(
            [FromBody] RemoveAmenityFromRoomCommand command,
            ISender _sender,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> AddAmenityToListing(
            [FromBody] AddAmenityToListingCommand command,
            ISender _sender,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> RemoveAmenityFromListing(
            [FromBody] RemoveAmenityFromListingCommand command,
            ISender _sender,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> Delete(
            string name,
            ISender _sender,
            CancellationToken ct = default)
        {
            var command = new DeleteAmenityCommand(name);
            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }
    }
}
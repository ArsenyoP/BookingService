using Booking.Application.Helpers.Room;
using Booking.Application.UseCases.Listing.CreateListing;
using Booking.Application.UseCases.Listing.DeleteListing;
using Booking.Application.UseCases.Listing.GetAllListings;
using Booking.Application.UseCases.Listing.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Endpoints
{
    public static class ListingEndpoints
    {
        public static void MapListingEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/listing")
                           .RequireRateLimiting("fixed")
                           .WithTags("Listings");

            group.MapGet("/", GetAll);
            group.MapGet("/{id:guid}", GetById);

            group.MapPost("/", CreateListing)
                 .RequireAuthorization()
                 .RequireRateLimiting("write-limiter");

            group.MapDelete("/{id:guid}", DeleteListing);
        }


        private static async Task<IResult> GetAll(
            [FromQuery] ListingQueryObject queryObject,
            ISender _sender,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetAllListingsQuery(queryObject), ct);

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

        private static async Task<IResult> CreateListing(
            [FromBody] CreateListingCommand command,
            ISender _sender,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Created($"api/listing/{result.Value}", result.Value)
                : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> DeleteListing(
            Guid id,
            ISender _sender,
            CancellationToken ct = default)
        {
            var command = new DeleteListingCommand(id);
            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }
    }
}
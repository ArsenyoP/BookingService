using Booking.Application.DTOs.Bookings;
using Booking.Application.UseCases.Booking.DeleteBooking;
using Booking.Application.UseCases.Bookings.CancelBooking;
using Booking.Application.UseCases.Bookings.ConfirmBooking;
using Booking.Application.UseCases.Bookings.CreateBooking;
using Booking.Application.UseCases.Bookings.GetAllBookings;
using Booking.Application.UseCases.Bookings.GetById;
using Booking.Application.UseCases.Bookings.GetByRoomId;
using Booking.Application.UseCases.Bookings.GetByUserId;
using Booking.Application.UseCases.Bookings.IsRoomAvailable;
using Booking.Infrastructure.ExtensionMethods;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Booking.API.Endpoints
{
    public static class BookingEndpoints
    {
        public static void MapBookingEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/bookings")
                .RequireRateLimiting("fixed")
                .WithTags("Bookings");

            group.MapGet("/", GetAll);
            group.MapGet("/{id:guid}", GetById);
            group.MapGet("/room/{id:guid}", GetByRoomId);
            group.MapGet("/user/{id:guid}", GetByUserId);
            group.MapGet("/roomBool/{id:guid}", IsRoomAvailable);
            group.MapGet("/confirm", ConfirmBooking);

            group.MapPost("/", Create)
                 .RequireAuthorization()
                 .RequireRateLimiting("write-limiter");

            group.MapDelete("/{id:guid}", Delete)
                .RequireAuthorization();


            group.MapPost("/cancel/{bookingId:guid}", Cancel)
                 .RequireAuthorization();
        }

        private static async Task<IResult> GetAll(
            ISender _sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetAllBookingsQuery(page, pageSize), ct);

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

        private static async Task<IResult> GetByRoomId(
            Guid id,
            ISender _sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetByRoomIdQuery(id, page, pageSize), ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> GetByUserId(
            Guid id,
            ISender _sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetByUserIdQuery(id, page, pageSize), ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> IsRoomAvailable(
            Guid id,
            [FromQuery] DateOnly start,
            [FromQuery] DateOnly end,
            ISender _sender,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(new IsRoomAvailableQuery(id, start, end), ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> Create(
            [FromBody] CreateBookingDto createDto,
            ClaimsPrincipal user,
            ISender _sender,
            CancellationToken ct)
        {
            var userId = user.GetUserID();

            var command = new CreateBookingCommand(createDto, Guid.Parse(userId));

            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Created($"/api/bookings/{result.Value}", result.Value)
                : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> Delete(
            Guid id,
            ISender _sender,
            CancellationToken ct)
        {
            var command = new DeleteBookingCommand(id);

            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> ConfirmBooking(
            [FromQuery] string token,
            ISender _sender,
            CancellationToken ct)
        {
            var command = new ConfirmBookingCommand(token);

            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok() : Results.BadRequest();
        }

        private static async Task<IResult> Cancel(
            [FromRoute] Guid bookingId,
            ClaimsPrincipal user,
            ISender _sender,
            CancellationToken ct)
        {
            var userId = user.GetUserID();
            var command = new CancelBookingCommand(bookingId, userId);

            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok() : Results.BadRequest();
        }
    }
}


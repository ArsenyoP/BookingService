using Booking.Application.DTOs.Reviews;
using Booking.Application.UseCases.Reviews.CreateReview;
using Booking.Application.UseCases.Reviews.DeleteReview;
using Booking.Application.UseCases.Reviews.GetAllReviews;
using Booking.Application.UseCases.Reviews.GetById;
using Booking.Application.UseCases.Reviews.GetByTargetId;
using Booking.Application.UseCases.Reviews.GetReviewsByUserId;
using Booking.Application.UseCases.Reviews.UpdateReview;
using Booking.Infrastructure.ExtensionMethods;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Booking.API.Endpoints
{
    public static class ReviewEndpoints
    {
        public static void MapReviewEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/reviews")
                           .RequireRateLimiting("fixed")
                           .WithTags("Reviews");

            group.MapPost("/", CreateReview).RequireAuthorization();
            group.MapPatch("/edit/{targetId:guid}", UpdateReview).RequireAuthorization();
            group.MapGet("/", GetAll);

            // Застосування Output Cache у Minimal API
            group.MapGet("/{targetId:guid}", GetByTargetId).CacheOutput("PublicReviews");

            group.MapGet("/details/{id:guid}", GetById);
            group.MapGet("/user/{userId:guid}", GetByUserId);
            group.MapDelete("/delete/{targetId:guid}", Delete).RequireAuthorization();
        }

        // --- Методи-обробники (Handlers) ---

        private static async Task<IResult> CreateReview(
            [FromBody] CreateReviewDto reviewDto,
            ClaimsPrincipal user,
            ISender _sender,
            CancellationToken ct = default)
        {
            var userId = user.GetUserID();
            var command = new CreateReviewCommand(reviewDto, userId);

            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> UpdateReview(
            [FromBody] UpdateReviewDto updateDto,
            [FromRoute] Guid targetId,
            ClaimsPrincipal user,
            ISender _sender,
            CancellationToken ct = default)
        {
            var userId = user.GetUserID();
            var command = new UpdateReviewCommand(updateDto, Guid.Parse(userId), targetId);

            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> GetAll(
            ISender _sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetAllReviewsQuery(page, pageSize), ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        }

        private static async Task<IResult> GetByTargetId(
            [FromRoute] Guid targetId,
            ISender _sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetReviewsByTargetIdQuery(page, pageSize, targetId), ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        }

        private static async Task<IResult> GetById(
            [FromRoute] Guid id,
            ISender _sender,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetReviewByIdQuery(id), ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Error);
        }

        private static async Task<IResult> GetByUserId(
            [FromRoute] Guid userId,
            ISender _sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetReviewsByUserIdQuery(page, pageSize, userId), ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }

        private static async Task<IResult> Delete(
            [FromRoute] Guid targetId,
            ClaimsPrincipal user,
            ISender _sender,
            CancellationToken ct = default)
        {
            var userId = user.GetUserID();
            var command = new DeleteReviewCommand(Guid.Parse(userId), targetId);

            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        }
    }
}
using Booking.Application.DTOs.Reviews;
using Booking.Application.UseCases.Reviews.CreateReview;
using Booking.Application.UseCases.Reviews.GetAllReviews;
using Booking.Application.UseCases.Reviews.GetById;
using Booking.Application.UseCases.Reviews.GetByTargetId;
using Booking.Application.UseCases.Reviews.GetReviewsByUserId;
using Booking.Infrastructure.ExtensionMethods;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Booking.API.Controllers
{
    [ApiController]
    [EnableRateLimiting("fixed")]
    [Route("api/reviews")]
    public class ReviewControllers(ISender _sender) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto reviewDto, CancellationToken ct = default)
        {
            var userId = User.GetUserID();

            var command = new CreateReviewCommand(reviewDto, userId);
            var result = await _sender.Send(command);

            return result.IsSuccess ? Ok(result.Value)
                : BadRequest(result.Error);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetAllReviewsQuery(page, pageSize));

            return result.IsSuccess
                ? Ok(result.Value)
                : NotFound(result.Error);
        }

        [HttpGet("{targetId:guid}")]
        public async Task<IActionResult> GetAll([FromRoute] Guid targetId, [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetReviewsByTargetIdQuery(page, pageSize, targetId));

            return result.IsSuccess
                ? Ok(result.Value)
                : NotFound(result.Error);
        }

        [HttpGet("details/{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var result = await _sender.Send(new GetReviewByIdQuery(id), ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : NotFound(result.Error);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetByUserId(
            [FromRoute] Guid userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetReviewsByUserIdQuery(page, pageSize, userId), ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }

    }
}

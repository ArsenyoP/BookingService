using Booking.Application.UseCases.Reviews.GetAllReviews;
using Booking.Application.UseCases.Reviews.GetByTargetId;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Booking.API.Controllers
{
    [ApiController]
    [EnableRateLimiting("fixed")]
    [Route("api/reviews")]
    public class ReviewControllers(ISender _sender) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetAllReviewsQuery(page, pageSize));

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }

        [HttpGet("{targetId:guid}")]
        public async Task<IActionResult> GetAll([FromRoute] Guid targetId, [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetReviewsByTargetIdQuery(page, pageSize, targetId));

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }

    }
}

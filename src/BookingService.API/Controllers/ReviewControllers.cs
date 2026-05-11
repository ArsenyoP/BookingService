using Booking.Application.UseCases.Reviews.GetAllReviews;
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
            var command = new GetAllReviewsQuery(page, pageSize);

            var result = await _sender.Send(command, ct);
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }
    }
}

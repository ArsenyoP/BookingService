using Booking.Application.UseCases.Listing.CreateListing;
using Booking.Application.UseCases.Listing.DeleteListing;
using Booking.Application.UseCases.Listing.GetAllListings;
using Booking.Application.UseCases.Listing.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Booking.API.Controllers
{
    [ApiController]
    [EnableRateLimiting("fixed")]
    [Route("api/listing")]
    public class ListingController(ISender _sender) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetAllListingsQuery(page, pageSize), ct);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetByIdQuery(id), ct);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [EnableRateLimiting("write-limiter")]
        [HttpPost]
        public async Task<IActionResult> CreateListing([FromBody] CreateListingCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);
            return result.IsSuccess ? Created($"api/listing/{result.Value}", result.Value) : BadRequest(result.Error);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteListing(Guid id, CancellationToken ct)
        {
            var command = new DeleteListingCommand(id);

            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }
    }
}

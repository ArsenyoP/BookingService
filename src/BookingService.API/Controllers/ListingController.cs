using Booking.Application.UseCases.Listing.CreateListing;
using Booking.Application.UseCases.Listing.GetAllListings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [ApiController]
    [Route("api/listing")]
    public class ListingController(ISender _sender) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var result = await _sender.Send(new GetAllListingsQuery(page, pageSize), ct);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> CreateListing([FromBody] CreateListingCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);
            return result.IsSuccess ? Created($"api/listing/{result.Value}", result.Value) : BadRequest(result.Error);
        }

    }
}

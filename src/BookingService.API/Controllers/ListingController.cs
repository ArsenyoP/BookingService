using Booking.Application.UseCases.Listing.CreateListing;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [ApiController]
    [Route("api/listing")]
    public class ListingController(ISender _sender) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateListing([FromBody] CreateListingCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);
            return result.IsSuccess ? Created($"api/listing/{result.Value}", result.Value) : BadRequest(result.Error);
        }

    }
}

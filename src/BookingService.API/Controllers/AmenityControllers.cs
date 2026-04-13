using Booking.Application.UseCases.Amenities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [ApiController]
    [Route("api/amenities")]
    public class AmenityControllers(ISender _sender) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> CreateAmenity([FromBody] CreateAmenityCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Created($"/api/amenities/{result.Value}", result.Value)
                : BadRequest(result.Error);
        }

    }
}

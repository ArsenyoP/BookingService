using Booking.Application.UseCases.Amenities;
using Booking.Application.UseCases.Amenities.AddAmenityToListing;
using Booking.Application.UseCases.Amenities.AddAmenityToRoom;
using Booking.Application.UseCases.Amenities.DeleteAmenity;
using Booking.Application.UseCases.Amenities.GetAllAmenities;
using Booking.Application.UseCases.Amenities.RemoveAmenityFromListing;
using Booking.Application.UseCases.Amenities.RemoveAmenityFromRoom;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Booking.API.Controllers
{
    [ApiController]
    [EnableRateLimiting("fixed")]
    [Route("api/amenities")]
    public class AmenityControllers(ISender _sender) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        {
            var command = new GetAllAmenitiesQuery(page, pageSize);

            var result = await _sender.Send(command, ct);
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }


        [HttpPost]
        public async Task<IActionResult> CreateAmenity([FromBody] CreateAmenityCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Created($"/api/amenities/{result.Value}", result.Value)
                : BadRequest(result.Error);
        }

        [HttpPost("addToRoom")]
        public async Task<IActionResult> AddAmenityToRoom([FromBody] AddAmenityToRoomCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }

        [Authorize]
        [HttpPost("removeFromRoom")]
        public async Task<IActionResult> RemoveAmenityFromRoom([FromBody] RemoveAmenityFromRoomCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }

        [HttpPost("addToListing")]
        public async Task<IActionResult> AddAmenityToListing([FromBody] AddAmenityToListingCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }

        [Authorize]
        [HttpPost("RemoveFromListing")]
        public async Task<IActionResult> RemoveAmenityFromListing([FromBody] RemoveAmenityFromListingCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name, CancellationToken ct)
        {
            var command = new DeleteAmenityCommand(name);

            var result = await _sender.Send(command, ct);
            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }
    }
}

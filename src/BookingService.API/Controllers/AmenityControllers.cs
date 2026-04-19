using Booking.Application.UseCases.Amenities;
using Booking.Application.UseCases.Amenities.AddAmenityToListing;
using Booking.Application.UseCases.Amenities.AddAmenityToRoom;
using Booking.Application.UseCases.Amenities.RemoveAmenityFromListing;
using Booking.Application.UseCases.Amenities.RemoveAmenityFromRoom;
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

        [HttpPost("addToRoom")]
        public async Task<IActionResult> AddAmenityToRoom([FromBody] AddAmenityToRoomCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }

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

        [HttpPost("RemoveFromListing")]
        public async Task<IActionResult> RemoveAmenityFromListing([FromBody] RemoveAmenityFromListingCommand command, CancellationToken ct)
        {
            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }
    }
}

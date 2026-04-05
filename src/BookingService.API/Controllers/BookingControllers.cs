using MediatR;
using Microsoft.AspNetCore.Mvc;
using Booking.Application.UseCases;
using Booking.Application.UseCases.Bookings.GetAllBookings;
using Booking.Domain.Common;

namespace Booking.API.Controllers
{

    [ApiController]
    [Route("api/bookings")]
    public class BookingControllers : ControllerBase
    {
        private readonly ISender _sender;

        public BookingControllers(ISender sender)
        {
            _sender = sender;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _sender.Send(new GetAllBookingsQuery(), ct);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

    }
}

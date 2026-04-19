using Booking.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.UseCases.Amenities.RemoveAmenityFromRoom
{
    public sealed record RemoveAmenityFromRoomCommand(Guid roomId, string amenityName) : ICommand<Guid>;

}

using Booking.Application.Abstractions;
using Booking.Application.DTOs.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.UseCases.Room.GetByListingId
{
    public sealed record GetByListingIdQuery(int Page, int PageSize, Guid listingId) : IQuery<IReadOnlyList<RoomResponseDto>>;

}

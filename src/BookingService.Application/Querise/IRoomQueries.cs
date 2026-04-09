using Booking.Application.DTOs.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Queries
{
    public interface IRoomQueries
    {
        Task<IReadOnlyList<RoomResponseDto>> GetPagedAsyncListingTitle(int page, int pageSize, CancellationToken ct = default);
    }
}

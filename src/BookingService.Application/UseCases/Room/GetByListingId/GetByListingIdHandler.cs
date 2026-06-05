using Booking.Application.Abstractions;
using Booking.Application.DTOs.Rooms;
using Booking.Application.Interfaces.IQueries;
using Booking.Application.Queries;
using Booking.Domain.Common;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;

namespace Booking.Application.UseCases.Room.GetByListingId
{
    public class GetByListingIdHandler(IRoomQueries _roomQueries,
        IListingQueries _listingQueries, IListingRepository _listingRepository) : IQueryHandler<GetByListingIdQuery, IReadOnlyList<RoomResponseDto>>
    {
        public async Task<Result<IReadOnlyList<RoomResponseDto>>> Handle(GetByListingIdQuery request, CancellationToken ct = default)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

            if (await _listingRepository.GetByIdWithAmenities(request.listingId) is null)
            {
                return Result<IReadOnlyList<RoomResponseDto>>.Failure(ListingErrors.NotFound);
            }

            var result = await _roomQueries.GetByListingIdAsync(request.listingId, page, pageSize, ct);

            return Result<IReadOnlyList<RoomResponseDto>>.Success(result);
        }
    }
}

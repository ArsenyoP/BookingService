using Booking.Application.Abstractions;
using Booking.Application.DTOs.Listings;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;
using Booking.Domain.Errors;

namespace Booking.Application.UseCases.Listing.GetById
{
    public sealed class GetByIdHandler(IListingQueries _listingQueries) : IQueryHandler<GetByIdQuery, ListingResponseDto>,
        ICachableQuery
    {
        public Guid Id { get; set; }

        public string Key => $"listing:{Id}";

        public TimeSpan Expiration => TimeSpan.FromSeconds(300);

        public async Task<Result<ListingResponseDto>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _listingQueries.GetByIdAsync(request.id);

            if (result is null)
            {
                return Result<ListingResponseDto>.Failure(ListingErrors.NotFound);
            }

            return Result<ListingResponseDto>.Success(result);
        }
    }
}

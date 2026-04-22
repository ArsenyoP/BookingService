using Booking.Application.Abstractions;
using Booking.Application.Interfaces;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Domain.Errors;

namespace Booking.Application.UseCases.Amenities.DeleteAmenity
{
    public sealed class DeleteBookingHandler(
        IAmenityRepository _amenityRepository,
        IAmenityQueries _amenityQueries,
        IUnitOfWork _unitOfWork) : ICommandHandler<DeleteAmenityCommand, string>
    {
        public async Task<Result<string>> Handle(DeleteAmenityCommand request, CancellationToken ct)
        {
            var amenity = await _amenityQueries.GetByNameAsync(request.Name);
            if (amenity is null)
            {
                return Result<string>.Failure(AmenityErrors.NotFound);
            }

            _amenityRepository.Delete(amenity);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<string>.Success(amenity.Name);
        }
    }
}

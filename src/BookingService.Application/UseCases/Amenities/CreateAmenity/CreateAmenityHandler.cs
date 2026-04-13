using Booking.Application.Abstractions;
using Booking.Application.Interfaces;
using Booking.Domain.Common;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.UseCases.Amenities.CreateAmenity
{
    public sealed class CreateAmenityHandler(IAmenityRepository _amenityRepository,
        IUnitOfWork _unitOfWork) : ICommandHandler<CreateAmenityCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(CreateAmenityCommand request, CancellationToken ct)
        {
            var result = Amenity.Create(request.Name, request.Category);

            if (!result.IsSuccess || result is null)
            {
                return Result<Guid>.Failure(AmenityErrors.CreationProblem);
            }

            _amenityRepository.Add(result.Value!);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<Guid>.Success(result.Value!.Id);
        }
    }
}

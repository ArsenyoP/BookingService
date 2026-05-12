using Booking.Application.Abstractions;
using Booking.Application.Interfaces;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.UseCases.Reviews.CreateReview
{
    public sealed class CreateReviewHandler(IReviewRepository _reviewRepo,
        IReviewQueries _reviewQueries, IUnitOfWork _unitOfWork) : ICommandHandler<CreateReviewCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(CreateReviewCommand request, CancellationToken ct)
        {
            var userIdGuid = Guid.Parse(request.userId);
            if (await _reviewQueries.HasReviewByUserAsync(userIdGuid, request.createReviewDto.TargetId, ct))
            {
                return Result<Guid>.Failure(ReviewErrors.AllreadyCreated);
            }

            bool hasBooking = request.createReviewDto.TargetType switch
            {
                ReviewsTargetType.Room => await _reviewQueries.HasRoomBooking(userIdGuid, request.createReviewDto.TargetId, ct),
                ReviewsTargetType.Listing => await _reviewQueries.HasListingBooking(userIdGuid, request.createReviewDto.TargetId, ct),
                _ => false
            };

            if (!hasBooking)
            {
                return Result<Guid>.Failure(ReviewErrors.NoBooking);
            }

            var reviewResult = Review.Create(userIdGuid, request.createReviewDto.TargetId,
                request.createReviewDto.TargetType, request.createReviewDto.Score, request.createReviewDto.Text);

            if (!reviewResult.IsSuccess) return Result<Guid>.Failure(reviewResult.Error);


            var strategy = _unitOfWork.CreateExecutingStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

                try
                {
                    _reviewRepo.Add(reviewResult.Value!);
                    await _unitOfWork.SaveChangesAsync(ct);

                    await _reviewQueries.AddedReviewToTarget(
                        request.createReviewDto.TargetId,
                        request.createReviewDto.Score,
                        request.createReviewDto.TargetType,
                        _unitOfWork.GetCurrentTransaction()!,
                        ct);

                    await _unitOfWork.CommitAsync(ct);

                    return Result<Guid>.Success(reviewResult.Value!.Id);
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackAsync(ct);
                    throw;
                }
            });

        }
    }
}

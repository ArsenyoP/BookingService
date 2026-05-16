using Booking.Application.Abstractions;
using Booking.Application.Interfaces;
using Booking.Application.Interfaces.IQueries;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Common;
using Booking.Domain.Enums;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.UseCases.Reviews.DeleteReview
{
    public sealed class DeleteReviewHandler(IReviewQueries _reviewQueries,
        IReviewRepository _reviewRepo, IUnitOfWork _unitOfWork,
        IOutputCacheStore _outputCache, ICacheService _cacheService) : ICommandHandler<DeleteReviewCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(DeleteReviewCommand request, CancellationToken ct)
        {
            var review = await _reviewQueries.GetReviewByUserAndTargetId(request.UserId, request.TargetId, ct);

            if (review is null)
            {
                return Result<Guid>.Failure(ReviewErrors.NotFound);
            }

            var strategy = _unitOfWork.CreateExecutingStrategy();

            var result = await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _unitOfWork.BeginTransactionAsync(ct);
                try
                {
                    _reviewRepo.Delete(review);
                    await _unitOfWork.SaveChangesAsync();

                    await _reviewQueries.RemovedReviewFromTarget(
                        request.TargetId,
                        review.Score,
                        review.TargetType,
                        _unitOfWork.GetCurrentTransaction()!,
                        ct);

                    await _unitOfWork.CommitAsync(ct);
                    return Result<Guid>.Success(review.Id);
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackAsync(ct);
                    throw;
                }
            });


            if (result.IsSuccess)
            {
                await _outputCache.EvictByTagAsync($"target_{request.TargetId.ToString().ToLowerInvariant()}", ct);
            }

            //invalidates room/listing distributed cache
            if (review.TargetType == ReviewsTargetType.Room)
            {
                await _cacheService.RemoveAsync($"room:{review.TargetId}", ct);
            }

            if (review.TargetType == ReviewsTargetType.Listing)
            {
                await _cacheService.RemoveAsync($"listing:{review.TargetId}", ct);
            }

            return result;
        }
    }
}

using Booking.Application.Abstractions;
using Booking.Application.DTOs.Reviews;
using Booking.Application.Interfaces;
using Booking.Application.Interfaces.IQueries;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Common;
using Booking.Domain.Enums;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.UseCases.Reviews.UpdateReview
{
    public sealed class UpdateReviewHandler(IReviewQueries _reviewQueries,
        IReviewRepository _reviewRepo, IUnitOfWork _unitOfWork,
        IOutputCacheStore _outputCache, ICacheService _cacheService) : ICommandHandler<UpdateReviewCommand, ReviewResponseDto>
    {
        public async Task<Result<ReviewResponseDto>> Handle(UpdateReviewCommand request, CancellationToken ct)
        {
            var review = await _reviewRepo.GetReviewByUserIdAndTargetId(request.UserId, request.TargetId, ct);

            if (review is null) return Result<ReviewResponseDto>.Failure(ReviewErrors.NotFound);
            if (review.IsEdited) return Result<ReviewResponseDto>.Failure(ReviewErrors.AlreadyEdited);

            var targetType = review.TargetType;

            var reviewDto = await _reviewQueries.GetById(review.Id, ct);

            var strategy = _unitOfWork.CreateExecutingStrategy();

            var result = await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

                try
                {
                    var targetScore = request.UpdateDto.Score ?? review.Score;
                    var targetText = request.UpdateDto.Text ?? review.Text;

                    var oldScore = review.Score;

                    var updateResult = review.Update(targetScore, targetText);

                    if (!updateResult.IsSuccess)
                    {
                        return Result<ReviewResponseDto>.Failure(updateResult.Error);
                    }

                    if (targetScore != oldScore)
                    {
                        await _reviewQueries.UpdatedReviewScoreOnTarget(
                            review.TargetId,
                            targetScore,
                            oldScore,
                            review.TargetType,
                            _unitOfWork.GetCurrentTransaction()!,
                            ct);
                    }


                    await _unitOfWork.SaveChangesAsync(ct);
                    await _unitOfWork.CommitAsync(ct);

                    reviewDto = reviewDto with
                    {
                        Score = review.Score,
                        Text = review.Text,
                        IsEdited = review.IsEdited
                    };

                    return Result<ReviewResponseDto>.Success(reviewDto);
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackAsync(ct);
                    throw;
                }
            });

            if (result.IsSuccess)
            {
                await _outputCache.EvictByTagAsync($"target_{review.TargetId.ToString().ToLowerInvariant()}", ct);
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

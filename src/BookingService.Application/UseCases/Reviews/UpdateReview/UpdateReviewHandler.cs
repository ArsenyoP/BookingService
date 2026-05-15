using Booking.Application.Abstractions;
using Booking.Application.DTOs.Reviews;
using Booking.Application.Interfaces;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Common;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace Booking.Application.UseCases.Reviews.UpdateReview
{
    public sealed class UpdateReviewHandler(IReviewQueries _reviewQueries,
        IReviewRepository _reviewRepo, IUnitOfWork _unitOfWork, IOutputCacheStore _outputCache) : ICommandHandler<UpdateReviewCommand, ReviewResponseDto>
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
                    if (request.UpdateDto.Score.HasValue && request.UpdateDto.Score.Value != review.Score)
                    {
                        await _reviewQueries.UpdatedReviewScoreOnTarget(
                            review.TargetId,
                            request.UpdateDto.Score.Value,
                            review.Score,
                            review.TargetType,
                            _unitOfWork.GetCurrentTransaction()!,
                            ct);

                        review.UpdateScore(request.UpdateDto.Score.Value);

                        reviewDto = reviewDto with { Score = request.UpdateDto.Score.Value, IsEdited = true };
                    }

                    if (request.UpdateDto.Text != null)
                    {
                        review.UpdateText(request.UpdateDto.Text);
                        reviewDto = reviewDto with { Text = request.UpdateDto.Text, IsEdited = true };
                    }

                    await _unitOfWork.SaveChangesAsync(ct);
                    await _unitOfWork.CommitAsync(ct);

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

            return result;
        }
    }
}

using Booking.Domain.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Booking.Infrastructure.BackgroundJobs
{
    [DisallowConcurrentExecution]
    public class CleanExpiredRefreshTokenJob(IRefreshTokenRepository _refreshRepo,
        ILogger<CleanExpiredRefreshTokenJob> _logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Quartz Job: Starting to clear invalid refresh tokens");
            try
            {
                await _refreshRepo.CleanExpiredTokens(context.CancellationToken);

                _logger.LogInformation("Quartz Job: Invalid refresh tokens cleared successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Quartz Job: An error occurred while clearing refresh tokens.");

                throw new JobExecutionException(ex) { RefireImmediately = true };
            }
        }
    }
}

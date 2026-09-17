using HandMade.Application.Features.Orders.Orchestrators.ConfirmDeliveryAction;
using HandMade.Application.Features.Orders.Queries.GetOrdersDueForAutoRelease;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HandMade.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Releases escrow for orders the artist finished but the client never confirmed.
    /// Without this an unresponsive buyer would leave the artist money held forever.
    ///
    /// Interval is configurable via Escrow:SweepIntervalMinutes; the grace period
    /// itself (Escrow:AutoReleaseDays) is applied when the artist marks the work
    /// complete and stamps Order.AutoReleaseAt.
    /// </summary>
    public class EscrowAutoReleaseService(
        IServiceScopeFactory _scopeFactory,
        IConfiguration _configuration,
        ILogger<EscrowAutoReleaseService> _logger) : BackgroundService
    {
        private const int DefaultSweepIntervalMinutes = 60;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var intervalMinutes = _configuration.GetValue<int?>("Escrow:SweepIntervalMinutes")
                                  ?? DefaultSweepIntervalMinutes;

            if (intervalMinutes < 1)
                intervalMinutes = DefaultSweepIntervalMinutes;

            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(intervalMinutes));

            do
            {
                try
                {
                    await SweepAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    // A failed sweep must never take the host down — the next tick retries.
                    _logger.LogError(ex, "Escrow auto-release sweep failed.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task SweepAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var due = await mediator.Send(new GetOrdersDueForAutoReleaseQuery(), cancellationToken);

            if (!due.IsSuccess || due.Data is null || due.Data.Count == 0)
                return;

            foreach (var orderId in due.Data)
            {
                var result = await mediator.Send(
                    new ConfirmDeliveryOrchestrator(orderId, null), cancellationToken);

                if (result.IsSuccess)
                    _logger.LogInformation("Auto-released escrow for order {OrderId}.", orderId);
                else
                    _logger.LogWarning(
                        "Auto-release skipped order {OrderId}: {ErrorCode}.", orderId, result.ErrorCode);
            }
        }
    }
}

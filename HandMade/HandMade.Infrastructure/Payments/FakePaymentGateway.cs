using HandMade.Application.Interfaces;
using HandMade.Application.Shared;

namespace HandMade.Infrastructure.Payments
{
    /// <summary>
    /// Development stand-in for a real provider. Succeeds for every charge, so the
    /// whole order flow is demoable without any gateway account or keys.
    ///
    /// To exercise the failure paths, charge an amount whose piastres are .99
    /// (e.g. 100.99) — the charge is declined. Refunds always succeed.
    /// </summary>
    public class FakePaymentGateway : IPaymentGateway
    {
        private const decimal DeclineTriggerFraction = 0.99m;

        public Task<GatewayResult> ChargeAsync(
            decimal amount,
            string currency,
            string idempotencyKey,
            CancellationToken ct = default)
        {
            if (amount <= 0)
                return Task.FromResult(GatewayResult.Failure("Amount must be greater than zero."));

            if (amount - decimal.Truncate(amount) == DeclineTriggerFraction)
                return Task.FromResult(GatewayResult.Failure("Card declined (simulated)."));

            return Task.FromResult(GatewayResult.Success($"FAKE-CH-{Guid.NewGuid():N}"));
        }

        public Task<GatewayResult> RefundAsync(
            string providerRef,
            decimal amount,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(providerRef))
                return Task.FromResult(GatewayResult.Failure("Missing provider reference."));

            return Task.FromResult(GatewayResult.Success($"FAKE-RF-{Guid.NewGuid():N}"));
        }
    }
}

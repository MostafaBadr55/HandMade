namespace HandMade.Application.Interfaces
{
    using HandMade.Application.Shared;

    /// <summary>
    /// The seam between the order flow and whatever payment provider is wired up.
    /// Implemented by FakePaymentGateway in development and by a real provider
    /// (Stripe, PayPal, Paymob) in production — the order handlers never change.
    ///
    /// Note this models a *charge*, not an authorization hold. Card authorizations
    /// expire in roughly a week, long before a handmade order is finished, so the
    /// platform takes the money up front and tracks the escrow itself via
    /// Payment.EscrowStatus.
    /// </summary>
    public interface IPaymentGateway
    {
        /// <summary>
        /// Charges the client. <paramref name="idempotencyKey"/> must be stable for a
        /// given order so a retried request cannot take the money twice.
        /// </summary>
        Task<GatewayResult> ChargeAsync(
            decimal amount,
            string currency,
            string idempotencyKey,
            CancellationToken ct = default);

        /// <summary>
        /// Returns money to the client — used when a later step of the accept
        /// orchestrator fails, and by the refund flow.
        /// </summary>
        Task<GatewayResult> RefundAsync(
            string providerRef,
            decimal amount,
            CancellationToken ct = default);
    }
}

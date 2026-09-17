namespace HandMade.Application.Shared
{
    /// <summary>
    /// Outcome of a single call to the payment provider. Deliberately provider-agnostic:
    /// <paramref name="ProviderRef"/> is whatever the gateway calls its transaction id
    /// (Stripe PaymentIntent id, PayPal capture id, a fake ref in development).
    /// </summary>
    public record GatewayResult(bool Succeeded, string? ProviderRef, string? FailureReason)
    {
        public static GatewayResult Success(string providerRef) => new(true, providerRef, null);

        public static GatewayResult Failure(string reason) => new(false, null, reason);
    }
}

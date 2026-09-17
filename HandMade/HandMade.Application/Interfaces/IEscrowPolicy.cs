namespace HandMade.Application.Interfaces
{
    /// <summary>
    /// How long the platform holds escrow after the artist marks an order complete
    /// before the auto-release sweep releases it unprompted. Kept behind an
    /// interface (like IPaymentGateway) so the Application layer never takes a
    /// direct dependency on IConfiguration.
    /// </summary>
    public interface IEscrowPolicy
    {
        int AutoReleaseDays { get; }
    }
}

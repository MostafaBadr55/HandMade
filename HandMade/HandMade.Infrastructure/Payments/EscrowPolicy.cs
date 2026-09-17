using HandMade.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace HandMade.Infrastructure.Payments
{
    /// <summary>
    /// Reads Escrow:AutoReleaseDays from configuration, defaulting to 7 when it is
    /// missing. EscrowAutoReleaseService reads the matching Escrow:SweepIntervalMinutes
    /// itself; this is only the grace period applied when an order is marked complete.
    /// </summary>
    public class EscrowPolicy(IConfiguration configuration) : IEscrowPolicy
    {
        private const int DefaultAutoReleaseDays = 7;

        public int AutoReleaseDays
        {
            get
            {
                var days = configuration.GetValue<int?>("Escrow:AutoReleaseDays") ?? DefaultAutoReleaseDays;
                return days < 0 ? DefaultAutoReleaseDays : days;
            }
        }
    }
}

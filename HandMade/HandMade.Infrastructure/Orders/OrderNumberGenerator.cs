using HandMade.Application.Interfaces;

namespace HandMade.Infrastructure.Orders
{
    /// <summary>
    /// Produces references like HM-20260909-A1B2C3D4. Date-prefixed so orders sort
    /// and read naturally; the random tail keeps the unique index happy without a
    /// round-trip to the database.
    /// </summary>
    public class OrderNumberGenerator : IOrderNumberGenerator
    {
        public string Generate()
        {
            var suffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            return $"HM-{DateTime.UtcNow:yyyyMMdd}-{suffix}";
        }
    }
}

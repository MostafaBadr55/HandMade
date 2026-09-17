namespace HandMade.Application.Interfaces
{
    /// <summary>
    /// Produces the human-facing order reference. Order.OrderNumber carries a
    /// unique index, so generation lives behind this interface rather than being
    /// hand-rolled at each call site.
    /// </summary>
    public interface IOrderNumberGenerator
    {
        string Generate();
    }
}

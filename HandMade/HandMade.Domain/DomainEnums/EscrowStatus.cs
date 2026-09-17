namespace HandMade.Domain.DomainEnums
{
    /// <summary>
    /// Where the money sits between the client paying and the artist being paid.
    /// The platform holds the funds; this is tracked in our own Payment table,
    /// not at the gateway (card authorizations expire long before a handmade
    /// order is finished).
    /// </summary>
    public enum EscrowStatus
    {
        None,
        Held,
        Released,
        Refunded
    }
}

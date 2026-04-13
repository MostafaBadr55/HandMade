namespace HandMade.Domain.DomainEnums
{
    public enum OrderStatus
    {
        SellerPending,     
        BuyerPending,        
        InProgress,          
        CompletedBySeller,   
        Delivered,          
        Cancelled,           
        Refunded             
    }
}
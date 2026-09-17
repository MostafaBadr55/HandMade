using HandMade.Domain.DomainEnums;

namespace HandMade.ViewModels.Orders.Requests
{
    public class AcceptOrderQuoteRequestVM
    {
        public PaymentMethod Method { get; set; } = PaymentMethod.CreditCard;
    }
}

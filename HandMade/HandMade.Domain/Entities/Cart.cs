namespace HandMade.Domain.Entities
{
    public class Cart : BaseModel
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
 
        public DateTime? CheckedOutAt { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}

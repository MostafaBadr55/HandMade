namespace ArteEva.Models
{
    public class Cart : BaseModel
    {
        public int UserId { get; set; }
        public User User { get; set; }
 
        public DateTime? CheckedOutAt { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}

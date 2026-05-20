
namespace HandMade.Domain.Entities
{
    public class ShopFollower : BaseModel
    {
        public Guid ShopId { get; set; }
        public Guid UserId { get; set; }

        // Navigation Properties
        public Shop Shop { get; set; }
    }
}

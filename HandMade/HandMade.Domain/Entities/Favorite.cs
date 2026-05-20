using System;

namespace HandMade.Domain.Entities
{
    public class Favorite : BaseModel
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }

        // Navigation Properties
        public Product Product { get; set; }
    }
}

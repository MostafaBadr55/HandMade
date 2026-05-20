using System;
using System.ComponentModel.DataAnnotations;

namespace HandMade.Domain.Entities
{
    public abstract class BaseModel
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}

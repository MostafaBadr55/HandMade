namespace HandMade.Domain.Entities
{
    public class UserRole 
    {
        public Guid Id { get; set; }
        // Navigation Properties
        public User User { get; set; }
        public Role Role { get; set; }
    }
}

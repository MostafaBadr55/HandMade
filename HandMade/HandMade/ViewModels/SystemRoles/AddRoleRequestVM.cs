using HandMade.Domain.DomainEnums;

namespace HandMade.ViewModels.SystemRoles
{
    public class AddRoleRequestVM
    {
        public AssignedRole Role { get; set; }
        public string Description { get; set; }
    }
}

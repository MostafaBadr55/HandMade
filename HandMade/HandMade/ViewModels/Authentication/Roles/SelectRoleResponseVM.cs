using Azure.Core;
using HandMade.Domain.DomainEnums;

namespace HandMade.ViewModels.Authentication.Roles
{
    public class SelectRoleResponseVM
    {
        public AssignedRole Role { get; set; }
        public string Username { get; set; }
        public string Message => $"Role '{Role}' assigned to user '{Username}' successfully";
    }
}

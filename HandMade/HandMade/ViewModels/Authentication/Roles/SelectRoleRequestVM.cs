using HandMade.Domain.DomainEnums;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Authentication.Roles
{
    public class SelectRoleRequestVM
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public AssignedRole SelectedRole { get; set; } 
    }
}

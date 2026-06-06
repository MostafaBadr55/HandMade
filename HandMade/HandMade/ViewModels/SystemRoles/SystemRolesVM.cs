namespace HandMade.ViewModels.SystemRoles
{
    public class SystemRolesVM
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }

        public static SystemRolesVM Create(string name, string description )
        {
            return new SystemRolesVM
            {
                RoleName = name,
                RoleDescription = description
            };
        }
    }
}

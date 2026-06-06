using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.CQRS.RolesManagement.DTOs
{
    public class SystemRoleDTO
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        private SystemRoleDTO() { }

        public static SystemRoleDTO Create(string name, string description)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            return new SystemRoleDTO
            {
                Name = name,
                Description = description
            };
        }

    }
}

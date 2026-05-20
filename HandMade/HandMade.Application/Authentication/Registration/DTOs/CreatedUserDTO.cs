using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Authentication.Registration.DTOs
{
    public class CreatedUserDTO
    {
        public Guid UserId { get; set; }
        public bool Created { get; set; } = false;
        public IList<string> Errors { get; set; }
        
        public CreatedUserDTO(List<string> errors, bool created = false)
        {
            Created = created;
            Errors = errors;
        }

        public CreatedUserDTO(Guid userId)
        {
            Created = true;
            Errors = new List<string>();
            UserId = userId;
        }
    }
}

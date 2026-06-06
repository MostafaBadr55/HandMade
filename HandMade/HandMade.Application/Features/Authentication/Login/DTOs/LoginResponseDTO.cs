using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.CQRS.Authentication.Login.DTOs
{
    public class LoginResponseDTO
    {
        public string? Token { get; set; }
        public Guid UserId { get; set; }
    }
}

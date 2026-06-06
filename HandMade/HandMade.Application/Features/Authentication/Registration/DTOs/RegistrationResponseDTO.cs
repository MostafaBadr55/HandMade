using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.CQRS.Authentication.Registration.DTOs
{
    public class RegistrationResponseDTO
    {
        public string Token { get; set; }
        public Guid UserId { get; set; }
    }
}

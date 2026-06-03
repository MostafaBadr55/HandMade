using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Authentication.Login
{
    public class LoginRequestVM
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string password { get; set; }

    }
}

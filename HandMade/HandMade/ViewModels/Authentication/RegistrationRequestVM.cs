using System.ComponentModel.DataAnnotations;

namespace HandMade.ViewModels.Authentication
{
    public class RegistrationRequestVM
    {
        public string Username { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string? AddressLable { get; set; }
        public string? Address { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string? Phone { get; set; }

    }
}

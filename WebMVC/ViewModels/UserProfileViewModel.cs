using Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace WebMVC.ViewModels
{
    public class UserProfileViewModel
    {
        public string Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        [RegularExpression(@"^(\d{10})$", ErrorMessage = "Invalid Phone Number")]
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? ProfilePictureUrl { get; set; }

    }
}

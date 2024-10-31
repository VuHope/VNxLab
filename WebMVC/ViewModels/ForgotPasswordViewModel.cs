using System.ComponentModel.DataAnnotations;

namespace WebMVC.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

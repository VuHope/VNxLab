using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        [NotMapped]
        public string? Role { get; set; }
        public virtual ICollection<Contact>? Contacts { get; set; }
        public virtual Portfolio? Portfolio { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Rating>? Ratings { get; set; }
        public virtual ICollection<ResearchProduct>? ResearchProducts { get; set; }
    }
}

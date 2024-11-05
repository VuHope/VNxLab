using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    public class ResearchProduct
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? CoverImg { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public string? Content { get; set; }
        public string? UrlHandle { get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Rating>? Rating { get; set; }
        public virtual ICollection<ResearchProductCategory>? ResearchProductCategories { get; set; }

    }
}

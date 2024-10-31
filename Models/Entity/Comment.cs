using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int? ProductId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? ApplicationUser { get; set; }
        [ForeignKey("ProductId")]
        public virtual ResearchProduct? ResearchProduct { get; set; }
    }
}

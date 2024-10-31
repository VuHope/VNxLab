using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Models.Enum;

namespace Models.Entity
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int? ProductId { get; set; }
        public RatingPoint Rate { get; set; }
        public DateTime? CreatedAt { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser? ApplicationUser { get; set; }
        [ForeignKey("ProductId")]
        public virtual ResearchProduct? ResearchProduct { get; set; }
    }
}

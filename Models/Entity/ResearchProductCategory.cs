using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    public class ResearchProductCategory
    {
        [Key]
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
        [ForeignKey("ProductId")]
        public virtual ResearchProduct? ResearchProduct { get; set; }
    }
}

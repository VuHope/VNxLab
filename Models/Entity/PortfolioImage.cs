using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Models.Entity
{
    public class PortfolioImage
    {
        [Key]
        public int Id { get; set; }
        public int? PortfolioId { get; set; }
        public string? Path { get; set; }
        [ForeignKey("PortfolioId")]
        public virtual Portfolio? Portfolio { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Models.Entity
{
    public class PortfolioImage
    {
        [Key]
        public int Id { get; set; }
        public int PortfolioId { get; set; } 
        public string ImageUrl { get; set; } 

        public Portfolio Portfolio { get; set; }
    }
}

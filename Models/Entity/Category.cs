using System.ComponentModel.DataAnnotations;

namespace Models.Entity
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<ResearchProductCategory>? ResearchProductCategories { get; set; }
    }
}

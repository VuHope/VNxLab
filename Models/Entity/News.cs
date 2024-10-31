using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    public class News
    {
        [Key]
        public int Id { get; set; }
        public string? Heading { get; set; } 
        public string? PageTitle { get; set; }
        public string? Content { get; set; }
        public string? ShortDescription { get; set; }
        public string? NewsImage { get; set; }
        public string? UrlHandle { get; set; }
        public DateTime DatePublished { get; set; }
    }
}

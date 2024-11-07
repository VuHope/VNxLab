using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Models.Entity
{
    public class Portfolio
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int Popularity { get; set; }
        public List<string> Images { get; set; }
        public string VideoUrl { get; set; }
        public DateTime DateCreated { get; set; }

        public Portfolio()
        {
            Images = new List<string>();
        }
    }
}

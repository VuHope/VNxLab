using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Entity
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? AdminId { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Message { get; set; }
        public string? Answer { get; set; }
        public int Status { get; set; } = 0;// 0 - chưa trả lời, 1 - đã trả lời, 2 - từ chối
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [ForeignKey("UserId")]
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}

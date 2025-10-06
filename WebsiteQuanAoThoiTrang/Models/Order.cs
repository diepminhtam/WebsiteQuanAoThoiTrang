using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteQuanAoThoiTrang.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
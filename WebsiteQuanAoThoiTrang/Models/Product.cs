using System.ComponentModel.DataAnnotations;

namespace WebsiteQuanAoThoiTrang.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
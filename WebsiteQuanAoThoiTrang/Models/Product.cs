using System.ComponentModel.DataAnnotations;

namespace WebsiteQuanAoThoiTrang.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Tên sản phẩm phải từ 1 đến 200 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá là bắt buộc.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
        public decimal Price { get; set; }

        [StringLength(500, ErrorMessage = "URL ảnh không được vượt quá 500 ký tự.")]
        public string ImageUrl { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng kho phải lớn hơn hoặc bằng 0.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc.")]
        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;
    }
}
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebsiteQuanAoThoiTrang.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Họ tên phải từ 1 đến 100 ký tự.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vai trò là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Vai trò không được vượt quá 50 ký tự.")]
        public string Role { get; set; } = "Customer";
    }
}
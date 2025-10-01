using Microsoft.AspNetCore.Identity;

namespace WebsiteQuanAoThoiTrang.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Role { get; set; } = "Customer"; // Default Customer
    }
}
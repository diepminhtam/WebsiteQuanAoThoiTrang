using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanAoThoiTrang.Data;  // Cho Include và ToListAsync

namespace WebsiteQuanAoThoiTrang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;  // Field chỉ định nghĩa 1 lần

        public HomeController(ApplicationDbContext context)
        {
            _context = context;  // Constructor chỉ 1 lần
        }

        public async Task<IActionResult> Index()
        {
            // Fix: Gán Include() vào biến var để infer IIncludableQueryable
            var query = _context.Products.Include(p => p.Category);  // Include trước để giữ type chainable
            var products = await query.Take(6).ToListAsync();  // Take sau Include, ToListAsync cuối
            return View(products);
        }

        public IActionResult Privacy()  // Method chỉ 1 lần
        {
            return View();
        }
    }
}
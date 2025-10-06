using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanAoThoiTrang.Data;
using WebsiteQuanAoThoiTrang.Models;

namespace WebsiteQuanAoThoiTrang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // FIX: Sử dụng var để infer type IIncludableQueryable từ Include
            var query = _context.Products.Include(p => p.Category);
            var products = await query.Take(6).ToListAsync();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
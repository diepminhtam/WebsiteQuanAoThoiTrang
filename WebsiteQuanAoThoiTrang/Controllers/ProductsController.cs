using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanAoThoiTrang.Data;  // Cho ApplicationDbContext
using WebsiteQuanAoThoiTrang.Models;  // Thêm dòng này để dùng Product, Category

namespace WebsiteQuanAoThoiTrang.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            // Include trước tất cả filter - giữ type IIncludableQueryable
            var query = _context.Products.Include(p => p.Category);

            // Tạo biến mới cho filter Where() để tránh conflict type
            IQueryable<Product> filteredQuery = query;  // Bây giờ Product được nhận diện nhờ using Models
            if (categoryId.HasValue)
            {
                filteredQuery = query.Where(p => p.CategoryId == categoryId.Value);  // Where sau Include, gán vào biến mới
            }

            // ToListAsync trên filteredQuery
            var products = await filteredQuery.ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }
    }
}
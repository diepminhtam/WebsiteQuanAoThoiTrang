using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanAoThoiTrang.Data;
using WebsiteQuanAoThoiTrang.Models;

namespace WebsiteQuanAoThoiTrang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        // GET: Details (xem chi tiết đơn hàng)
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                TempData["Error"] = "Đơn hàng không tồn tại.";
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // FIX: POST UpdateStatus (try-catch SaveChangesAsync, kiểm tra order)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                TempData["Error"] = "Đơn hàng không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                order.Status = status;
                await _context.SaveChangesAsync();  
                TempData["Success"] = "Cập nhật trạng thái thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật: " + ex.Message; 
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
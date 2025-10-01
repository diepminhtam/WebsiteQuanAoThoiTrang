using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanAoThoiTrang.Data;  // Cho ApplicationDbContext
using WebsiteQuanAoThoiTrang.Models;  // Cho Order, etc.
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace WebsiteQuanAoThoiTrang.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        // THÊM: GET Create - Hiển thị form thanh toán nếu giỏ không rỗng
        [HttpGet]
        public IActionResult Create()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = session.GetString("Cart");
            var cart = string.IsNullOrEmpty(cartJson) ? new List<CartController.CartItem>() : JsonSerializer.Deserialize<List<CartController.CartItem>>(cartJson);

            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Giỏ hàng rỗng. Vui lòng thêm sản phẩm trước khi đặt hàng.";
                return RedirectToAction("Index", "Cart");
            }

            return View();  // Trả về view form thanh toán
        }

        // POST Create - Xử lý đặt hàng
        [HttpPost]
        [ValidateAntiForgeryToken]  // Bảo mật form
        public async Task<IActionResult> Create(string address)  // Nhận địa chỉ từ form
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = session.GetString("Cart");
            var cart = JsonSerializer.Deserialize<List<CartController.CartItem>>(cartJson ?? "[]");

            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Giỏ hàng rỗng.";
                return RedirectToAction("Index", "Cart");
            }

            // Tạo order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(c => c.Quantity * c.Price),
                Status = "Pending"
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Thêm chi tiết đơn hàng và cập nhật stock
            foreach (var item in cart)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    _context.OrderDetails.Add(new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    });
                    product.Stock -= item.Quantity;
                }
            }

            await _context.SaveChangesAsync();
            session.Remove("Cart");  // Xóa giỏ sau khi đặt hàng
            TempData["Success"] = "Đặt hàng thành công!";
            return RedirectToAction("History");
        }

        public async Task<IActionResult> History()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.UserId == userId)
                .ToListAsync();
            return View(orders);
        }


        // POST: Hủy đơn hàng (chỉ Pending)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                TempData["Error"] = "Đơn hàng không tồn tại.";
                return RedirectToAction("History");
            }

            if (order.Status != "Pending")
            {
                TempData["Error"] = "Chỉ có thể hủy đơn hàng đang chờ xử lý.";
                return RedirectToAction("History");
            }

            // Hủy: Đổi status, trả stock
            order.Status = "Cancelled";
            foreach (var detail in order.OrderDetails)
            {
                detail.Product.Stock += detail.Quantity;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã hủy đơn hàng thành công!";
            return RedirectToAction("History");
        }
    }
}
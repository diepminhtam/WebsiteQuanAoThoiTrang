using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebsiteQuanAoThoiTrang.Data;
using WebsiteQuanAoThoiTrang.Models;

namespace WebsiteQuanAoThoiTrang.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = session.GetString("Cart");
            var cart = string.IsNullOrEmpty(cartJson) ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(cartJson);
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _context.Products.Find(productId);
            if (product == null || product.Stock < quantity)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại hoặc hết hàng." });
            }

            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = session.GetString("Cart");
            var cart = string.IsNullOrEmpty(cartJson) ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(cartJson);

            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                item.Quantity += quantity;
                if (item.Quantity > product.Stock)
                {
                    return Json(new { success = false, message = "Số lượng vượt quá tồn kho." });
                }
            }
            else
            {
                cart.Add(new CartItem { ProductId = productId, Quantity = quantity, Price = product.Price });
            }

            session.SetString("Cart", JsonSerializer.Serialize(cart));
            return Json(new { success = true, message = "Đã thêm vào giỏ!", totalItems = cart.Sum(c => c.Quantity) });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = session.GetString("Cart");
            var cart = string.IsNullOrEmpty(cartJson) ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(cartJson);

            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                session.SetString("Cart", JsonSerializer.Serialize(cart));
                return Json(new { success = true, message = "Đã xóa khỏi giỏ!", totalItems = cart.Sum(c => c.Quantity) });
            }

            return Json(new { success = false, message = "Item không tồn tại trong giỏ." });
        }

        public class CartItem
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebsiteQuanAoThoiTrang.Data;
using WebsiteQuanAoThoiTrang.Models;

namespace WebsiteQuanAoThoiTrang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalProducts = await _context.Products.CountAsync();
            ViewBag.TodayOrders = await _context.Orders.Where(o => o.OrderDate.Date == DateTime.Today).CountAsync();
            ViewBag.MonthlyRevenue = await _context.Orders
                .Where(o => o.OrderDate.Month == DateTime.Now.Month && o.OrderDate.Year == DateTime.Now.Year)
                .SumAsync(o => o.TotalAmount);
            ViewBag.NewUsers = await _context.Users.Where(u => u.Role == "Customer").CountAsync();

            var categoriesData = await _context.Categories
                .Select(c => new { Name = c.Name, Count = c.Products.Count })
                .ToListAsync();
            ViewBag.CategoryData = JsonSerializer.Serialize(categoriesData);

            var statusData = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();
            ViewBag.StatusData = JsonSerializer.Serialize(statusData);

            var revenueData = await _context.Orders
                .GroupBy(o => new { o.OrderDate.Month, o.OrderDate.Year })
                .Select(g => new { Month = g.Key.Month, Year = g.Key.Year, Revenue = g.Sum(o => o.TotalAmount) })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .Take(6)
                .ToListAsync();
            ViewBag.RevenueData = JsonSerializer.Serialize(revenueData);

            return View();
        }
    }
}
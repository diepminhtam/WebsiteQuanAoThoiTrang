using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebsiteQuanAoThoiTrang.Data;
using WebsiteQuanAoThoiTrang.Models;

namespace WebsiteQuanAoThoiTrang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index(string search, string role = "")
        {
            var users = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));
            }
            if (!string.IsNullOrEmpty(role) && role != "All")
            {
                users = users.Where(u => u.Role == role);
            }
            var userList = await users.ToListAsync();
            ViewBag.Search = search;
            ViewBag.Role = role;
            ViewBag.Roles = new[] { "All", "Admin", "Customer" };
            return View(userList);
        }

        // GET: Create (mở form thêm user)
        public IActionResult Create()
        {
            ViewBag.Roles = new[] { "Admin", "Customer" };
            return View();
        }

        // POST: Create (thêm user)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser model, string password)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    ModelState.AddModelError("", "Email đã tồn tại.");
                    ViewBag.Roles = new[] { "Admin", "Customer" };
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Role = model.Role
                };
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    TempData["Success"] = "Thêm user thành công!";
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            ViewBag.Roles = new[] { "Admin", "Customer" };
            return View(model);
        }

        // GET: Edit (mở form sửa user)
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            ViewBag.Roles = new[] { "Admin", "Customer" };
            return View(user);
        }

        // POST: Edit (cập nhật user)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser model)
        {
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return NotFound();

                user.FullName = model.FullName;
                user.Email = model.Email;
                user.UserName = model.Email;
                user.Role = model.Role;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    TempData["Success"] = "Cập nhật user thành công!";
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            ViewBag.Roles = new[] { "Admin", "Customer" };
            return View(model);
        }

        // FIX: POST Delete (kiểm tra tồn tại, không xóa chính mình, log TempData)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            // Không xóa chính mình
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == currentUserId)
            {
                TempData["Error"] = "Không thể xóa chính mình.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Xóa user thành công!";
            }
            else
            {
                TempData["Error"] = "Lỗi khi xóa user.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
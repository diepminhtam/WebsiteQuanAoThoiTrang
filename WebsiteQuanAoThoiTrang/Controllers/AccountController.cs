using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;  // THÊM: Để dùng List<Claim>
using WebsiteQuanAoThoiTrang.Models;

namespace WebsiteQuanAoThoiTrang.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Login for Customer
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login for Customer
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user != null)
                    {
                        // FIX: Tạo Claims chung cho FullName (cho cả Admin và Customer)
                        var claims = new List<Claim>
                        {
                            new Claim("FullName", user.FullName ?? "")
                        };
                        await _signInManager.RefreshSignInAsync(user);
                        await _signInManager.SignInWithClaimsAsync(user, false, claims);

                        // Redirect dựa trên Role
                        if (user.Role == "Admin")
                        {
                            return RedirectToAction("Index", "Admin", new { area = "Admin" });
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("", "Đăng nhập thất bại. Vui lòng kiểm tra email và mật khẩu.");
            return View();
        }

        // GET: Register for Customer
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register for Customer
        [HttpPost]
        public async Task<IActionResult> Register(ApplicationUser model, string password)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Role = "Customer"
                };
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    // Lưu FullName vào Claims khi SignIn
                    var claims = new List<Claim>
                    {
                        new Claim("FullName", user.FullName ?? "")
                    };
                    await _signInManager.SignInWithClaimsAsync(user, false, claims);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: Admin Login riêng
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }

        // POST: AdminLogin 
        [HttpPost]
        public async Task<IActionResult> AdminLogin(string email, string password)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user != null && user.Role == "Admin")
                    {
                        // Lưu FullName vào Claims
                        var claims = new List<Claim>
                        {
                            new Claim("FullName", user.FullName ?? "")
                        };
                        await _signInManager.RefreshSignInAsync(user);
                        await _signInManager.SignInWithClaimsAsync(user, false, claims);
                        return RedirectToAction("Index", "Admin", new { area = "Admin" });
                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            ModelState.AddModelError("", "Đăng nhập Admin thất bại. Vui lòng kiểm tra thông tin.");
            return View();
        }
    }
}
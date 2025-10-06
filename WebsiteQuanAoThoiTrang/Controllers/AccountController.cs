using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ email và mật khẩu.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    var claims = new List<Claim> { new Claim("FullName", user.FullName ?? "") };
                    await _signInManager.RefreshSignInAsync(user);
                    await _signInManager.SignInWithClaimsAsync(user, false, claims);

                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin", new { area = "Admin" });
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "Đăng nhập thất bại. Vui lòng kiểm tra email và mật khẩu.");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ApplicationUser model, string password)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    ModelState.AddModelError("", "Email đã tồn tại.");
                    return View(model);
                }

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
                    await _userManager.AddToRoleAsync(user, "Customer");

                    var claims = new List<Claim> { new Claim("FullName", user.FullName ?? "") };
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ email và mật khẩu.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null && user.Role == "Admin")
                {
                    var claims = new List<Claim> { new Claim("FullName", user.FullName ?? "") };
                    await _signInManager.RefreshSignInAsync(user);
                    await _signInManager.SignInWithClaimsAsync(user, false, claims);
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                }
                else
                {
                    ModelState.AddModelError("", "Tài khoản không phải Admin.");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", "Sai email hoặc mật khẩu.");
            }
            return View();
        }

        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(ApplicationUser model, string password)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    ModelState.AddModelError("", "Email đã tồn tại.");
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    Role = "Admin"
                };
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");

                    var claims = new List<Claim> { new Claim("FullName", user.FullName ?? "") };
                    await _signInManager.SignInWithClaimsAsync(user, false, claims);

                    TempData["Success"] = "Đăng ký Admin thành công! Bạn đã được đăng nhập.";
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
    }
}
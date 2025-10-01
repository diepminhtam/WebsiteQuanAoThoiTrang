using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanAoThoiTrang.Data;  // Namespace cho ApplicationDbContext (nếu ở Data)
using WebsiteQuanAoThoiTrang.Models;  // Cho ApplicationUser

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Cấu hình DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// THÊM CẤU HÌNH SESSION (FIX LỖI)
builder.Services.AddDistributedMemoryCache();  // In-memory cache cho Session (dev mode)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Thời gian hết hạn Session
    options.Cookie.HttpOnly = true;  // Bảo mật cookie
    options.Cookie.IsEssential = true;  // Cho phép cookie cần thiết
});

// THÊM IHttpContextAccessor cho CartController
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// THÊM USE SESSION VÀO PIPELINE (SAU UseRouting, TRƯỚC UseAuthentication)
app.UseSession();  // Enable Session middleware

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
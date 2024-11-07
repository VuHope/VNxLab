using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models.Entity;
using System.Globalization;
using WebMVC.Data;
using WebMVC.Repository;
using WebMVC.Repository.IRepository;
using WebMVC.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromSeconds(100);
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductImage, ProductImageRepository>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = new PathString("/Account/AccessDenied");
});
// Tùy chỉnh cấu hình password
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1; // Số ký tự đặc biệt yêu cầu trong password (mặc định là 1)
    options.Lockout.MaxFailedAccessAttempts = 3; // Số lần thử đăng nhập sai trước khi khóa tài khoản (lockoutOnFailure: true)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); // Thời gian khóa tài khoản 1 phút
});
// Configure localization options
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
// Cấu hình localization options
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new List<CultureInfo>
    {
        new CultureInfo("en-US"),
        new CultureInfo("vi-VN") // Thêm ngôn ngữ hỗ trợ tại đây
    };

    options.DefaultRequestCulture = new RequestCulture("en-US", "en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
SeedDatabase();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}
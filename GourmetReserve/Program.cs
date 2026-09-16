using GourmetReserve.Data;
using GourmetReserve.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------- Регистрация сервисов ----------
builder.Services.AddControllersWithViews();

// Регистрация ApplicationDbContext с провайдером PostgreSQL (Npgsql)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Учётные данные единственного администратора (секция "AdminUser" в appsettings.json)
builder.Services.Configure<AdminCredentialsOptions>(builder.Configuration.GetSection("AdminUser"));

// Антифорджери-токен для AJAX-запросов (booking.js отправляет его в заголовке,
// а не как поле формы, поэтому явно указываем имя заголовка).
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

// Куки-аутентификация для админ-панели.
// Для нескольких сотрудников/ролей замените на ASP.NET Core Identity.
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// ---------- Применение миграций и наполнение тестовыми данными ----------
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.SeedAsync(context);
}

// ---------- Конвейер middleware ----------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
using EmreCakmakoglu.Data;
using EmreCakmakoglu.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı Yolu (Hem Lokal Hem Sunucu İçin En Sağlıklı Yol)
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "EmreCakmakoglu.db");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// 2. Servislerin Eklenmesi
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// 3. İstek Hattı (Middleware) Yapılandırması
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// NOT: Plesk üzerinden HTTPS yönlendirmesi açıksa bu satırın kapalı kalması daha iyidir.
// app.UseHttpsRedirection(); 

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// 4. Varsayılan Rota
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 5. Veritabanı Otomasyonu (Migrate ve Admin Kontrolü)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        // Migrate() açık kalmalı: Verileri silmez, sadece yeni tablo/sütun eklediğinde günceller.
        context.Database.Migrate();

        // Eğer veritabanı tamamen boşsa Admin hesabı ekle
        if (!context.Admins.Any())
        {
            context.Admins.Add(new Admin
            {
                Username = "admin",
                Password = "1234"
            });
            context.SaveChanges();
        }

        // Test müziklerini artık eklemiyoruz çünkü gerçek veritabanın var.
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı işlemleri sırasında hata!");
    }
}

app.Run();
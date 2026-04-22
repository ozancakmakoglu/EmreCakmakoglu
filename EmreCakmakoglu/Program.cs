using EmreCakmakoglu.Data;
using EmreCakmakoglu.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
// 1. SQLite Bağlantı Ayarı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=EmreCakmakoglu.db"));

// 2. MVC Servislerini ekle
builder.Services.AddControllersWithViews();

// --- EKLEDİĞİMİZ KISIM: Session Ayarları ---
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 30 dakika hareketsizlikte oturum kapanır
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; 
});
builder.Services.AddHttpContextAccessor();
// ------------------------------------------

var app = builder.Build();

// 3. HTTP istek hattını yapılandır
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// --- EKLEDİĞİMİZ KISIM: Session Kullanımı ---
app.UseSession();
// ------------------------------------------

app.UseAuthorization();

// 4. Varsayılan URL rotası
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// --- VERİTABANI TOHUMLAMA (SEED DATA) ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    // Müzik Test Verileri
    if (!context.Musics.Any())
    {
        context.Musics.AddRange(
            new Music { Title = "Yalnız Gece", Year = "2018", ImageUrl = "https://via.placeholder.com/300", SpotifyUrl = "" },
            new Music { Title = "Ateş Hattı", Year = "2019", ImageUrl = "https://via.placeholder.com/300", SpotifyUrl = "" },
            new Music { Title = "Kayıp Şehir", Year = "2018", ImageUrl = "https://via.placeholder.com/300", SpotifyUrl = "" }
        );
    }

    // Admin Hesabı Oluşturma (Eğer yoksa)
    if (!context.Admins.Any())
    {
        context.Admins.Add(new Admin
        {
            Username = "admin",
            Password = "1234"
        });
    }

    context.SaveChanges();
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    // Sunucudaki DB'de eksik olan sütunları (Month gibi) otomatik ekler, verileri silmez.
    context.Database.Migrate();
}

app.Run();
app.Run();
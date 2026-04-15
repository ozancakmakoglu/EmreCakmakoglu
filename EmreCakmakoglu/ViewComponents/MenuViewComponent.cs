using EmreCakmakoglu.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class MenuViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    public MenuViewComponent(AppDbContext context) => _context = context;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        // 1. Aktif menüleri sırasıyla çekiyoruz
        var items = await _context.Menus
                                  .Where(x => x.IsActive)
                                  .OrderBy(x => x.Order)
                                  .ToListAsync();

        // 2. Veritabanındaki en güncel kitabı buluyoruz
        var latestBook = await _context.Books
                                       .OrderByDescending(b => b.Id)
                                       .FirstOrDefaultAsync();

        // 3. Menü listesini tarayıp "KİTAP" linkini manipüle ediyoruz
        if (latestBook != null)
        {
            foreach (var item in items)
            {
                // Menü başlığı "KİTAP" (büyük/küçük harf duyarsız) ise linki değiştir
                if (item.Title?.ToUpper() == "KİTAP" || item.Title?.ToUpper() == "BOOKS")
                {
                    item.Url = "/Home/BookDetail/" + latestBook.Id;
                }
            }
        }

        return View(items);
    }
}
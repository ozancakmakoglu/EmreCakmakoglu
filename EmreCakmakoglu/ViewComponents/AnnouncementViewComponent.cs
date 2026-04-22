using EmreCakmakoglu.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EmreCakmakoglu.Models;

namespace EmreCakmakoglu.ViewComponents
{
    public class AnnouncementViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        public AnnouncementViewComponent(AppDbContext context) => _context = context;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Id'ye göre tersten sırala (en son eklenen en üstte) ve sadece ilk 5'ini al
            var announcements = await _context.Announcements
                .OrderByDescending(x => x.Id)
                .Take(5)
                .ToListAsync();

            return View(announcements);
        }
    }
}
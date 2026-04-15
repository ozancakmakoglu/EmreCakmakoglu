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
            var items = await _context.Announcements
                .Where(x => x.IsActive)
                .OrderBy(x => x.Order)
                .Take(5)
                .ToListAsync();

            return View(items);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using EmreCakmakoglu.Data;
using EmreCakmakoglu.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EmreCakmakoglu.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult About()
        {
            return View();
        }

        // --- ANA SAYFA ---
        public IActionResult Index()
        {
            var musics = _context.Musics
                                 .OrderByDescending(m => m.Year)
                                 .ThenByDescending(m => m.Month)
                                 .Take(6)
                                 .ToList();

            ViewBag.Settings = _context.SiteSettings.FirstOrDefault();

            ViewBag.Blogs = _context.BlogPosts
                                   .OrderByDescending(b => b.CreatedDate)
                                   .Take(5)
                                   .ToList();

            ViewBag.LatestBook = _context.Books
                                        .OrderByDescending(b => b.CreatedDate)
                                        .FirstOrDefault();

            return View(musics);
        }

        // --- TÜM MÜZİKLER LİSTESİ ---
        public IActionResult Music()
        {
            var allMusic = _context.Musics
                                   .OrderByDescending(m => m.Year)
                                   .ThenByDescending(m => m.Month)
                                   .ToList();
            return View(allMusic);
        }

        // --- MÜZİK DETAY ---
        public IActionResult MusicDetail(int id)
        {
            var music = _context.Musics.Find(id);
            if (music == null) return NotFound();

            return View(music);
        }

        // --- TÜM BLOG YAZILARI LİSTESİ ---
        public IActionResult Blog()
        {
            var allBlogs = _context.BlogPosts
                                   .OrderByDescending(b => b.CreatedDate)
                                   .ToList();
            return View(allBlogs);
        }

        // --- BLOG DETAY ---
        public IActionResult BlogDetail(int id)
        {
            var post = _context.BlogPosts.Find(id);
            if (post == null) return NotFound();

            return View(post);
        }

        // --- KİTAP DETAY ---
        public IActionResult BookDetail(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();

            return View(book);
        }

        // --- TÜM KİTAPLAR ---
        public IActionResult Books()
        {
            var allBooks = _context.Books
                                   .OrderByDescending(b => b.CreatedDate)
                                   .ToList();
            return View(allBooks);
        }

        // --- İLETİŞİM SAYFASI ---
        public IActionResult Contact()
        {
            return View();
        }

        // --- İLETİŞİM FORMU ---
        [HttpPost]
        public async Task<IActionResult> SendMessage(ContactMessage model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                _context.ContactMessages.Add(model);
                await _context.SaveChangesAsync();

                TempData["MessageSent"] = "Mesajınız başarıyla gönderildi!";
                return RedirectToAction("Contact");
            }

            return View("Contact", model);
        }

        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrEmpty(q)) return RedirectToAction("Index");

            var query = q.ToLower();

            var viewModel = new SearchViewModel
            {
                SearchQuery = q,
                Blogs = await _context.BlogPosts
                    .Where(b => b.Title.ToLower().Contains(query) || b.Summary.ToLower().Contains(query))
                    .ToListAsync(),
                Musics = await _context.Musics
                    .Where(m => m.Title.ToLower().Contains(query) || m.Description.ToLower().Contains(query))
                    .ToListAsync(),
                Books = await _context.Books
                    .Where(b => b.Title.ToLower().Contains(query) || b.Subtitle.ToLower().Contains(query))
                    .ToListAsync()
            };

            return View(viewModel);
        }
        // Tüm Albümlerin Listesi
        public async Task<IActionResult> Gallery()
        {
            var albums = await _context.Albums.OrderByDescending(a => a.CreatedDate).ToListAsync();
            return View(albums);
        }

        // Albümün İçindeki Fotoğraflar
        public async Task<IActionResult> AlbumDetail(int id)
        {
            var album = await _context.Albums
                .Include(a => a.Images)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null) return NotFound();

            return View(album);
        }
    }
}
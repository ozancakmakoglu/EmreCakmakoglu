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

        public IActionResult MenuCreate()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
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
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/musics");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                music.ImageUrl = "/uploads/musics/" + fileName;
            }

            _context.Musics.Add(music);
            _context.SaveChanges();

            return RedirectToAction("MusicList");
        }

        public IActionResult MusicList()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View(_context.Musics.OrderByDescending(m => m.Year).ToList());
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

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/musics");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                existing.ImageUrl = "/uploads/musics/" + fileName;
            }

            existing.Title = music.Title;
            existing.Year = music.Year;
            existing.MusicType = music.MusicType;
            existing.SpotifyUrl = music.SpotifyUrl;
            existing.YoutubeUrl = music.YoutubeUrl;
            existing.AppleMusicUrl = music.AppleMusicUrl;
            existing.Description = music.Description;

            _context.SaveChanges();

            return RedirectToAction("MusicList");
        }

        // --- KİTAP ---
        public IActionResult BookList()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View(_context.Books.ToList());
        }

        // --- BLOG ---
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
        public async Task<IActionResult> BlogCreate(BlogPost post, IFormFile? imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/blogs");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

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
        public async Task<IActionResult> BlogEdit(BlogPost post, IFormFile? imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var existing = await _context.BlogPosts.FindAsync(post.Id);
            if (existing == null) return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/blogs");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

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

        // --- DUYURU ---
        public async Task<IActionResult> AnnouncementList()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View(await _context.Announcements.OrderBy(x => x.Order).ToListAsync());
        }

        public IActionResult AnnouncementCreate()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
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

        // --- AYARLAR ---
        public IActionResult Settings()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View(_context.SiteSettings.FirstOrDefault());
        }
        // --- ALBÜM YÖNETİMİ BAŞLANGIÇ ---

        // 1. Albüm Listesi
        public IActionResult AlbumList()
        {
            var albums = _context.Albums.ToList();
            return View(albums);
        }

        // 2. Yeni Albüm Oluştur (Sayfa)
        public IActionResult AlbumCreate()
        {
            return View();
        }

        // 3. Yeni Albüm Oluştur (İşlem)
        [HttpPost]
        public IActionResult AlbumCreate(Album album, IFormFile coverImage)
        {
            if (coverImage != null)
            {
                // Resim yükleme kodunu buraya ekleyeceğiz (Örn: /wwwroot/img/gallery/)
                // Şimdilik basitçe kaydedelim:
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(coverImage.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/gallery", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    coverImage.CopyTo(stream);
                }
                album.CoverImageUrl = "/img/gallery/" + fileName;
            }

            _context.Albums.Add(album);
            _context.SaveChanges();
            return RedirectToAction("AlbumList");
        }
        public IActionResult AlbumDetails(int id)
        {
            // Include kullanarak albümle birlikte fotoğrafları da çekiyoruz
            var album = _context.Albums
                .Include(a => a.Images)
                .FirstOrDefault(a => a.Id == id);

            if (album == null) return NotFound();

            return View(album);
        }
        [HttpPost]
        public IActionResult AddImageToAlbum(int AlbumId, string Caption, IFormFile photo)
        {
            if (photo != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/gallery", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }

                var newImage = new AlbumImage
                {
                    AlbumId = AlbumId,
                    ImageUrl = "/img/gallery/" + fileName,
                    Caption = Caption
                };

                _context.AlbumImages.Add(newImage);
                _context.SaveChanges();
            }

            return RedirectToAction("AlbumDetails", new { id = AlbumId });
        }
    }

}

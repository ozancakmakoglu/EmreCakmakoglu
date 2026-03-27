using Microsoft.EntityFrameworkCore;
using EmreCakmakoglu.Models;

namespace EmreCakmakoglu.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tablo tanımlamaları
        public DbSet<Music> Musics { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<SiteSettings> SiteSettings { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace EmreCakmakoglu.Models
{
    public class SiteSettings
    {
        [Key]
        public int Id { get; set; }

        // Tema
        public string? ActiveTheme { get; set; } // Örn: "site.css", "site2.css"

        // Hakkımda Alanı
        public string? AboutTitle { get; set; }
        public string? AboutText { get; set; }
        public string? AboutImageUrl { get; set; } // YENİ EKLENEN

        // Medya Ayarları
        public string? YoutubeVideoId { get; set; } // Örn: VqnQmjfkr_4
        public string? SpotifyEmbedUrl { get; set; } // Spotify embed linki
        public string? HeroImageUrl { get; set; } // Ana görsel

        // Sosyal Medya Linkleri
        public string? InstagramUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? YoutubeChannelUrl { get; set; }
        public string? SpotifyArtistUrl { get; set; }

        // Alt Kısım Metni
        public string? FooterText { get; set; }
    }
}
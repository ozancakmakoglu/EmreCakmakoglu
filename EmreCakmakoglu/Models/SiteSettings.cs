using System.ComponentModel.DataAnnotations;

namespace EmreCakmakoglu.Models
{
    public class SiteSettings
    {
        [Key]
        public int Id { get; set; }

        // Medya Ayarları
        public string? YoutubeVideoId { get; set; } // Örn: VqnQmjfkr_4
        public string? SpotifyEmbedUrl { get; set; } // Spotify'ın "Paylaş > Yerleştir" linki
        public string? HeroImageUrl { get; set; }// YENİ EKLENEN ALAN: Sanatçı Ana Görseli (Saydam PNG)

        // Sosyal Medya Linkleri
        public string? InstagramUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? YoutubeChannelUrl { get; set; }
        public string? SpotifyArtistUrl { get; set; }
        public string? ActiveTheme { get; set; } // Örn: "site.css", "site2.css"

        // Alt Kısım Metni (Copyright vb.)
        public string? FooterText { get; set; }
    }
}
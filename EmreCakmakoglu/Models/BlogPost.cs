using System.ComponentModel.DataAnnotations;

namespace EmreCakmakoglu.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Summary { get; set; } // Ana sayfada görünecek kısa özet
        public string? Content { get; set; }  // Zengin metin içeriği
        public string? ImageUrl { get; set; }
        public string? ExternalUrl { get; set; } // Boşsa içeriden okunur, doluysa dışarı gider.
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
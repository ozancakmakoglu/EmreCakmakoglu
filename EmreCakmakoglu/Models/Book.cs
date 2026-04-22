using System.ComponentModel.DataAnnotations;
namespace EmreCakmakoglu.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Subtitle { get; set; } // "Çok Yakında" veya "Yeni Roman" gibi
        public string? Description { get; set; } // Kitap özeti (CKEditor ile)
        public string? ImageUrl { get; set; } // Kapak görseli
        public string? BuyLink { get; set; } // Satın alma linki (D&R, Idefix vb.)
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}


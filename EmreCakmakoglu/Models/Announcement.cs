namespace EmreCakmakoglu.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string DateText { get; set; } // Örn: "Ocak 2026"
        public string Title { get; set; }    // Örn: "Trol"
        public string SubTitle { get; set; } // Örn: "(Can Yayınları)"
        public string? Url { get; set; } // Yeni eklenen link alanı
        public int Order { get; set; }       // Sıralama
        public bool IsActive { get; set; } = true;
    }
}

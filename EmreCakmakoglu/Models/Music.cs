namespace EmreCakmakoglu.Models
{
    public class Music
    {
        public int Id { get; set; }
        public string Title { get; set; } // Şarkı/Albüm Adı
        public string Year { get; set; }  // Çıkış Yılı (Örn: 2019)
        public string ImageUrl { get; set; } // Kapak Görseli Yolu
        public string? SpotifyUrl { get; set; } // Soru işareti boş bırakılabilir olmasını sağlar
        public string? Description { get; set; } // Şarkı sözleri veya hikayesi buraya gelecek
    }
}
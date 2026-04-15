using System;
using System.Collections.Generic;

namespace EmreCakmakoglu.Models
{
    public class Album
    {
        public int Id { get; set; }
        public string Title { get; set; } // Albüm Adı (Örn: Konser Hatıraları)
        public string Description { get; set; } // Kısa Açıklama
        public string? CoverImageUrl { get; set; } // Albüm Kapağı
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Bir albümün birden fazla fotoğrafı olabilir (İlişki)
        public List<AlbumImage> Images { get; set; } = new List<AlbumImage>();
    }
}
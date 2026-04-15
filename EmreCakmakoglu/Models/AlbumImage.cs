namespace EmreCakmakoglu.Models
{
    public class AlbumImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } // Fotoğrafın yolu
        public string? Caption { get; set; } // Fotoğrafın altına gelecek kısa not

        // Hangi albüme ait olduğu (Foreign Key)
        public int AlbumId { get; set; }
        public Album Album { get; set; }
    }
}
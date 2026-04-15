using System.Collections.Generic;

namespace EmreCakmakoglu.Models
{
    public class SearchViewModel
    {
        public string SearchQuery { get; set; }
        public List<Music> Musics { get; set; }
        public List<BlogPost> Blogs { get; set; }
        public List<Book> Books { get; set; } // Kitaplarda da arama yapmak istersen
    }
}
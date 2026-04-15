using System.ComponentModel.DataAnnotations;

namespace EmreCakmakoglu.Models
{
    public class Menu
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
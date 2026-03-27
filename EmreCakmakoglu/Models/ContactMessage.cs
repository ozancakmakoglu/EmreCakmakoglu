using System;
using System.ComponentModel.DataAnnotations;

namespace EmreCakmakoglu.Models
{
    public class ContactMessage
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Konu alanı zorunludur.")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Mesaj alanı zorunludur.")]
        public string MessageBody { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Okundu/Okunmadı takibi yapmak istersen:
        public bool IsRead { get; set; } = false;
    }
}
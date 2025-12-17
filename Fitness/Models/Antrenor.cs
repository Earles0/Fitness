using System.ComponentModel.DataAnnotations;

namespace Fitness.Models
{
    public class Antrenor
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir")]
        [Display(Name = "Ad Soyad")]
        public string AdSoyad { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Uzmanlık alanı zorunludur")]
        [StringLength(200)]
        [Display(Name = "Uzmanlık Alanı")]
        public string UzmanlikAlani { get; set; } = string.Empty;

        // Antrenörün çalıştığı salon
        [Required]
        public int SalonId { get; set; }
        public Salon? Salon { get; set; }

        // Many-to-Many ilişki için ara tablo
        public ICollection<AntrenorHizmet> AntrenorHizmetler { get; set; } = new List<AntrenorHizmet>();
        public ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();
        public ICollection<AntrenorMusaitlik> Musaitlikler { get; set; } = new List<AntrenorMusaitlik>();
    }
}

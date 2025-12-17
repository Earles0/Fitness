using System.ComponentModel.DataAnnotations;

namespace Fitness.Models
{
    public class Randevu
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tarih ve saat zorunludur")]
        [Display(Name = "Tarih ve Saat")]
        public DateTime TarihSaat { get; set; }
        
        [Display(Name = "Onaylandı mı?")]
        public bool OnaylandiMi { get; set; } = false;

        // İlişkiler
        [Required]
        public string UyeId { get; set; } = string.Empty;
        public AppUser? Uye { get; set; }

        [Required]
        public int AntrenorId { get; set; }
        public Antrenor? Antrenor { get; set; }

        [Required]
        public int HizmetId { get; set; }
        public Hizmet? Hizmet { get; set; }
    }
}

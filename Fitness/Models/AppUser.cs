using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Fitness.Models
{
    public class AppUser : IdentityUser
    {
        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir")]
        public string AdSoyad { get; set; } = string.Empty;
     
        [Range(50, 250, ErrorMessage = "Boy 50-250 cm arasında olmalıdır")]
        public double? Boy { get; set; }
        
        [Range(20, 300, ErrorMessage = "Kilo 20-300 kg arasında olmalıdır")]
        public double? Kilo { get; set; }
        
        [StringLength(50)]
        public string? VucutTipi { get; set; }
        
        // Navigation Property
        public ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();
    }
}

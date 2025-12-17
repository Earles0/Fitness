using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitness.Models
{
    public class Hizmet
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Hizmet adı zorunludur")]
        [StringLength(100, ErrorMessage = "Hizmet adı en fazla 100 karakter olabilir")]
        [Display(Name = "Hizmet Adı")]
        public string Ad { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Süre zorunludur")]
        [Range(15, 240, ErrorMessage = "Süre 15-240 dakika arasında olmalıdır")]
        [Display(Name = "Süre (Dakika)")]
        public int SureDakika { get; set; }
        
        [Required(ErrorMessage = "Ücret zorunludur")]
        [Range(0, 10000, ErrorMessage = "Ücret 0-10000 TL arasında olmalıdır")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Ücret")]
        public decimal Ucret { get; set; }
        
        // Hizmetin sunulduğu salon
        [Required]
        public int SalonId { get; set; }
        public Salon? Salon { get; set; }

        // Many-to-Many ilişki için ara tablo
        public ICollection<AntrenorHizmet> AntrenorHizmetler { get; set; } = new List<AntrenorHizmet>();
        public ICollection<Randevu> Randevular { get; set; } = new List<Randevu>();
    }
}

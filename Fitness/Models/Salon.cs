using System.ComponentModel.DataAnnotations;

namespace Fitness.Models
{
    public class Salon
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Salon adı zorunludur")]
        [StringLength(100, ErrorMessage = "Salon adı en fazla 100 karakter olabilir")]
        public string Ad { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Adres zorunludur")]
        [StringLength(250)]
        public string Adres { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Çalışma saatleri zorunludur")]
        [StringLength(100, ErrorMessage = "Örn: 08:00-22:00")]
        public string CalismaSaatleri { get; set; } = string.Empty;

        // Navigation Properties
        public ICollection<Antrenor> Antrenorler { get; set; } = new List<Antrenor>();
        public ICollection<Hizmet> Hizmetler { get; set; } = new List<Hizmet>();
    }
}

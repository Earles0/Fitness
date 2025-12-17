using System.ComponentModel.DataAnnotations;

namespace Fitness.Models
{
    public class AntrenorMusaitlik
    {
        public int Id { get; set; }
        
        [Required]
        public int AntrenorId { get; set; }
        public Antrenor? Antrenor { get; set; }
        
        [Required]
        [Display(Name = "Gün")]
        public DayOfWeek Gun { get; set; }
        
        [Required]
        [Display(Name = "Başlangıç Saati")]
        public TimeSpan BaslangicSaati { get; set; }
        
        [Required]
        [Display(Name = "Bitiş Saati")]
        public TimeSpan BitisSaati { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Fitness.ViewModels
{
    public class RandevuCreateViewModel
    {
        [Required(ErrorMessage = "Hizmet secimi gereklidir")]
        [Display(Name = "Hizmet")]
        public int HizmetId { get; set; }

        [Required(ErrorMessage = "Antrenor secimi gereklidir")]
        [Display(Name = "Antrenor")]
        public int AntrenorId { get; set; }

        [Required(ErrorMessage = "Tarih ve saat gereklidir")]
        [Display(Name = "Randevu Tarihi ve Saati")]
        public DateTime TarihSaat { get; set; }

        // Dropdown'lar icin
        public SelectList? Hizmetler { get; set; }
        public SelectList? Antrenorler { get; set; }
    }
}

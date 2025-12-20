using Fitness.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Fitness.ViewModels
{
    public class AntrenorCreateViewModel
    {
        [Required(ErrorMessage = "Ad Soyad gereklidir")]
        [StringLength(100)]
        [Display(Name = "Ad Soyad")]
        public string AdSoyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Uzmanlik alani gereklidir")]
        [StringLength(200)]
        [Display(Name = "Uzmanlik Alani")]
        public string UzmanlikAlani { get; set; } = string.Empty;

        [Required(ErrorMessage = "Salon secimi gereklidir")]
        [Display(Name = "Salon")]
        public int SalonId { get; set; }

        [Display(Name = "Verebildigi Hizmetler")]
        public List<int> SelectedHizmetIds { get; set; } = new List<int>();

        // Dropdown icin
        public SelectList? Salonlar { get; set; }
        public List<Hizmet>? AvailableHizmetler { get; set; }
    }
}

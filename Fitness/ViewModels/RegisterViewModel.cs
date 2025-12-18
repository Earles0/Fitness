using System.ComponentModel.DataAnnotations;

namespace Fitness.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad Soyad gereklidir")]
        [StringLength(100)]
        [Display(Name = "Ad Soyad")]
        public string AdSoyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email gereklidir")]
        [EmailAddress(ErrorMessage = "Gecerli bir email adresi giriniz")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sifre gereklidir")]
        [StringLength(100, ErrorMessage = "Sifre en az {2} karakter olmalidir", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Sifre")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Sifre Tekrar")]
        [Compare("Password", ErrorMessage = "Sifreler eslesmedi")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Range(50, 250, ErrorMessage = "Boy 50-250 cm arasinda olmalidir")]
        [Display(Name = "Boy (cm)")]
        public double? Boy { get; set; }

        [Range(20, 300, ErrorMessage = "Kilo 20-300 kg arasinda olmalidir")]
        [Display(Name = "Kilo (kg)")]
        public double? Kilo { get; set; }

        [StringLength(50)]
        [Display(Name = "Vucut Tipi")]
        public string? VucutTipi { get; set; }
    }
}

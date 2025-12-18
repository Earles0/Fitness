using System.ComponentModel.DataAnnotations;

namespace Fitness.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email gereklidir")]
        [EmailAddress(ErrorMessage = "Gecerli bir email adresi giriniz")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sifre gereklidir")]
        [DataType(DataType.Password)]
        [Display(Name = "Sifre")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Beni Hatirla")]
        public bool RememberMe { get; set; }
    }
}

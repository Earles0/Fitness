using Fitness.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Fitness.Controllers
{
    [Authorize(Roles = "Uye")]
    public class AIController : Controller
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        // GET: AI/EgzersizOnerisi
        public IActionResult EgzersizOnerisi()
        {
            return View(new EgzersizOneriViewModel());
        }

        // POST: AI/EgzersizOnerisi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EgzersizOnerisi(EgzersizOneriViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model.Oneri = await _aiService.GetEgzersizOnerisiAsync(
                    model.Hedef,
                    model.Yas,
                    model.Kilo,
                    model.Boy,
                    model.Cinsiyet
                );
                model.OneriOlusturuldu = true;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "AI servisi ile baglanti kurulamadi: " + ex.Message);
            }

            return View(model);
        }

        // GET: AI/GorselAnaliz
        public IActionResult GorselAnaliz()
        {
            return View(new GorselAnalizViewModel());
        }

        // POST: AI/GorselAnaliz
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GorselAnaliz(GorselAnalizViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.GorselDosya == null || model.GorselDosya.Length == 0)
            {
                ModelState.AddModelError("GorselDosya", "Lutfen bir gorsel yukleyin");
                return View(model);
            }

            // Dosya boyutu kontrolu (max 5MB)
            if (model.GorselDosya.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("GorselDosya", "Gorsel boyutu en fazla 5MB olabilir");
                return View(model);
            }

            // Dosya tipi kontrolu
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
            if (!allowedTypes.Contains(model.GorselDosya.ContentType))
            {
                ModelState.AddModelError("GorselDosya", "Sadece JPG, JPEG ve PNG formatinda gorsel yukleyebilirsiniz");
                return View(model);
            }

            try
            {
                // Gorseli byte array'e cevir
                using var memoryStream = new MemoryStream();
                await model.GorselDosya.CopyToAsync(memoryStream);
                byte[] imageData = memoryStream.ToArray();

                // AI analizini yap
                model.Analiz = await _aiService.GetGorselAnalizAsync(imageData, model.Hedef);
                model.AnalizTamamlandi = true;

                // Gorseli base64'e cevir (onizleme icin)
                model.GorselBase64 = Convert.ToBase64String(imageData);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Gorsel analizi sirasinda hata olustu: " + ex.Message);
            }

            return View(model);
        }
    }

    public class EgzersizOneriViewModel
    {
        [Required(ErrorMessage = "Hedef zorunludur")]
        [Display(Name = "Hedefiniz")]
        public string Hedef { get; set; } = "";

        [Required]
        [Range(15, 100, ErrorMessage = "Yas 15-100 arasinda olmalidir")]
        [Display(Name = "Yas")]
        public int Yas { get; set; }

        [Required]
        [Range(30, 300, ErrorMessage = "Kilo 30-300 kg arasinda olmalidir")]
        [Display(Name = "Kilo (kg)")]
        public decimal Kilo { get; set; }

        [Required]
        [Range(100, 250, ErrorMessage = "Boy 100-250 cm arasinda olmalidir")]
        [Display(Name = "Boy (cm)")]
        public decimal Boy { get; set; }

        [Required]
        [Display(Name = "Cinsiyet")]
        public string Cinsiyet { get; set; } = "Erkek";

        public string? Oneri { get; set; }
        public bool OneriOlusturuldu { get; set; }
    }

    public class GorselAnalizViewModel
    {
        [Required(ErrorMessage = "Hedef zorunludur")]
        [Display(Name = "Hedefiniz")]
        public string Hedef { get; set; } = "";

        [Required(ErrorMessage = "Gorsel yuklemek zorunludur")]
        [Display(Name = "Vucut Gorseli")]
        public IFormFile? GorselDosya { get; set; }

        public string? Analiz { get; set; }
        public bool AnalizTamamlandi { get; set; }
        public string? GorselBase64 { get; set; }
    }
}

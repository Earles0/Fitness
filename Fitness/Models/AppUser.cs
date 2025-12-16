namespace Fitness.Models
{
    using Microsoft.AspNetCore.Identity;

    public class AppUser : IdentityUser
    {
        public string AdSoyad { get; set; }
        // Yapay zeka için gerekli fiziksel bilgiler [cite: 31]
        public double? Boy { get; set; }
        public double? Kilo { get; set; }
        public string? VucutTipi { get; set; }
    }
}

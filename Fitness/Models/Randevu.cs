namespace Fitness.Models
{
    public class Randevu
    {
        public int Id { get; set; }
        public DateTime TarihSaat { get; set; }
        public bool OnaylandiMi { get; set; } // Onay mekanizması için [cite: 21]

        // İlişkiler
        public string UyeId { get; set; } // AppUser ile ilişkilendirilecek
        public AppUser Uye { get; set; }

        public int AntrenorId { get; set; }
        public Antrenor Antrenor { get; set; }

        public int HizmetId { get; set; }
        public Hizmet Hizmet { get; set; }
    }
}

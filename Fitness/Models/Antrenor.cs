namespace Fitness.Models
{
    public class Antrenor
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; }
        public string UzmanlikAlani { get; set; } 

        // Antrenörün çalıştığı salon
        public int SalonId { get; set; }
        public Salon Salon { get; set; }

        // Antrenörün verebildiği hizmetler (Çoka-çok ilişki gerekebilir)
        public ICollection<Hizmet> Hizmetler { get; set; }
        public ICollection<Randevu> Randevular { get; set; }
    }
}

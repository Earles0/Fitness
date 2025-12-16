namespace Fitness.Models
{
    public class Salon
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Adres { get; set; }
        public string CalismaSaatleri { get; set; } 

        // İlişkiler
        public ICollection<Antrenor> Antrenorler { get; set; }
        public ICollection<Hizmet> Hizmetler { get; set; }
    }
}

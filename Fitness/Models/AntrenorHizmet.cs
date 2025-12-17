    namespace Fitness.Models
{
    // Antrenör-Hizmet Many-to-Many ara tablosu
    public class AntrenorHizmet
    {
        public int AntrenorId { get; set; }
        public Antrenor? Antrenor { get; set; }

        public int HizmetId { get; set; }
        public Hizmet? Hizmet { get; set; }
    }
}

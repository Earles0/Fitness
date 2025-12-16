namespace Fitness.Models
{
    public class Hizmet
    {
        public int Id { get; set; }
        public string Ad { get; set; } 
        public int SureDakika { get; set; }
        public decimal Ucret { get; set; }

       
        public ICollection<Antrenor> Antrenorler { get; set; } 
    }
}

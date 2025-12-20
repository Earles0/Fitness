using Microsoft.AspNetCore.Mvc;

namespace Fitness.Services
{
    public interface IAIService
    {
        Task<string> GetEgzersizOnerisiAsync(string hedef, int yas, decimal kilo, decimal boy, string cinsiyet);
        Task<string> GetGorselAnalizAsync(byte[] imageData, string hedef);
    }
}

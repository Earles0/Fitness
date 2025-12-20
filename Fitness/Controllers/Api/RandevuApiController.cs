using Fitness.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandevuApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RandevuApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RandevuApi
        // Tüm randevuları listele
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var randevular = await _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .Select(r => new
                {
                    r.Id,
                    r.TarihSaat,
                    r.OnaylandiMi,
                    Uye = new
                    {
                        r.Uye.Id,
                        r.Uye.AdSoyad,
                        r.Uye.Email
                    },
                    Antrenor = new
                    {
                        r.Antrenor.Id,
                        r.Antrenor.AdSoyad
                    },
                    Hizmet = new
                    {
                        r.Hizmet.Id,
                        r.Hizmet.Ad,
                        r.Hizmet.Ucret
                    }
                })
                .OrderByDescending(r => r.TarihSaat)
                .ToListAsync();

            return Ok(randevular);
        }

        // GET: api/RandevuApi/ByUye?email=test@example.com
        // Üyeye göre randevuları listele (LINQ)
        [HttpGet("ByUye")]
        public async Task<IActionResult> GetByUye([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Email adresi belirtilmedi" });
            }

            var randevular = await _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .Where(r => r.Uye.Email == email) // LINQ filtreleme
                .Select(r => new
                {
                    r.Id,
                    r.TarihSaat,
                    r.OnaylandiMi,
                    Antrenor = r.Antrenor.AdSoyad,
                    Hizmet = r.Hizmet.Ad,
                    Ucret = r.Hizmet.Ucret
                })
                .OrderByDescending(r => r.TarihSaat)
                .ToListAsync();

            if (!randevular.Any())
            {
                return NotFound(new { message = $"{email} için randevu bulunamadı" });
            }

            return Ok(randevular);
        }

        // GET: api/RandevuApi/ByTarih?tarih=2024-01-15
        // Tarihe göre randevuları listele (LINQ)
        [HttpGet("ByTarih")]
        public async Task<IActionResult> GetByTarih([FromQuery] DateTime tarih)
        {
            var randevular = await _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .Where(r => r.TarihSaat.Date == tarih.Date) // LINQ filtreleme
                .Select(r => new
                {
                    r.Id,
                    Saat = r.TarihSaat.ToString("HH:mm"),
                    r.OnaylandiMi,
                    Uye = r.Uye.AdSoyad,
                    Antrenor = r.Antrenor.AdSoyad,
                    Hizmet = r.Hizmet.Ad
                })
                .OrderBy(r => r.Saat)
                .ToListAsync();

            if (!randevular.Any())
            {
                return NotFound(new { message = $"{tarih:dd.MM.yyyy} tarihinde randevu bulunamadı" });
            }

            return Ok(randevular);
        }

        // GET: api/RandevuApi/OnayBekleyen
        // Onay bekleyen randevuları listele (LINQ)
        [HttpGet("OnayBekleyen")]
        public async Task<IActionResult> GetOnayBekleyen()
        {
            var randevular = await _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .Where(r => !r.OnaylandiMi) // LINQ filtreleme
                .Select(r => new
                {
                    r.Id,
                    r.TarihSaat,
                    Uye = r.Uye.AdSoyad,
                    UyeEmail = r.Uye.Email,
                    Antrenor = r.Antrenor.AdSoyad,
                    Hizmet = r.Hizmet.Ad
                })
                .OrderBy(r => r.TarihSaat)
                .ToListAsync();

            return Ok(new
            {
                count = randevular.Count,
                data = randevular
            });
        }
    }
}

using Fitness.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AntrenorApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AntrenorApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AntrenorApi
        // Tüm antrenörleri listele
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var antrenorler = await _context.Antrenorler
                .Include(a => a.Salon)
                .Include(a => a.AntrenorHizmetler)
                    .ThenInclude(ah => ah.Hizmet)
                .Include(a => a.Musaitlikler) //  Düzeltildi
                .Select(a => new
                {
                    a.Id,
                    a.AdSoyad,
                    a.UzmanlikAlani,
                    Salon = new
                    {
                        a.Salon.Id,
                        a.Salon.Ad,
                        a.Salon.Adres
                    },
                    Hizmetler = a.AntrenorHizmetler.Select(ah => new
                    {
                        ah.Hizmet.Id,
                        ah.Hizmet.Ad,
                        ah.Hizmet.SureDakika,
                        ah.Hizmet.Ucret
                    }).ToList(),
                    Musaitlikler = a.Musaitlikler.Select(am => new
                    {
                        Gun = am.Gun.ToString(),
                        BaslangicSaati = am.BaslangicSaati.ToString(@"hh\:mm"),
                        BitisSaati = am.BitisSaati.ToString(@"hh\:mm")
                    }).ToList()
                })
                .ToListAsync();

            return Ok(antrenorler);
        }

        // GET: api/AntrenorApi/5
        // ID'ye göre antrenör getir
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var antrenor = await _context.Antrenorler
                .Include(a => a.Salon)
                .Include(a => a.AntrenorHizmetler)
                    .ThenInclude(ah => ah.Hizmet)
                .Include(a => a.Musaitlikler)
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.AdSoyad,
                    a.UzmanlikAlani,
                    Salon = new
                    {
                        a.Salon.Id,
                        a.Salon.Ad,
                        a.Salon.Adres
                    },
                    Hizmetler = a.AntrenorHizmetler.Select(ah => new
                    {
                        ah.Hizmet.Id,
                        ah.Hizmet.Ad,
                        ah.Hizmet.SureDakika,
                        ah.Hizmet.Ucret
                    }).ToList(),
                    Musaitlikler = a.Musaitlikler.Select(am => new
                    {
                        Gun = am.Gun.ToString(),
                        BaslangicSaati = am.BaslangicSaati.ToString(@"hh\:mm"),
                        BitisSaati = am.BitisSaati.ToString(@"hh\:mm")
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (antrenor == null)
            {
                return NotFound(new { message = "Antrenör bulunamadı" });
            }

            return Ok(antrenor);
        }

        // GET: api/AntrenorApi/ByUzmanlik?alan=Yoga
        // Uzmanlık alanına göre antrenörleri filtrele (LINQ)
        [HttpGet("ByUzmanlik")]
        public async Task<IActionResult> GetByUzmanlik([FromQuery] string alan)
        {
            if (string.IsNullOrWhiteSpace(alan))
            {
                return BadRequest(new { message = "Uzmanlık alanı belirtilmedi" });
            }

            var antrenorler = await _context.Antrenorler
                .Include(a => a.Salon)
                .Where(a => a.UzmanlikAlani.Contains(alan)) // LINQ filtreleme
                .Select(a => new
                {
                    a.Id,
                    a.AdSoyad,
                    a.UzmanlikAlani,
                    Salon = a.Salon.Ad
                })
                .ToListAsync();

            if (!antrenorler.Any())
            {
                return NotFound(new { message = $"'{alan}' uzmanlık alanında antrenör bulunamadı" });
            }

            return Ok(antrenorler);
        }

        // GET: api/AntrenorApi/ByGun?gun=Monday
        // Belirli günde müsait olan antrenörleri listele (LINQ)
        [HttpGet("ByGun")]
        public async Task<IActionResult> GetByMusaitGun([FromQuery] string gun)
        {
            if (string.IsNullOrWhiteSpace(gun) || !Enum.TryParse<DayOfWeek>(gun, true, out var dayOfWeek))
            {
                return BadRequest(new { message = "Geçerli bir gün belirtilmedi (Monday, Tuesday, vb.)" });
            }

            var antrenorler = await _context.AntrenorMusaitlikler
                .Include(am => am.Antrenor)
                    .ThenInclude(a => a.Salon)
                .Where(am => am.Gun == dayOfWeek) // LINQ filtreleme
                .Select(am => new
                {
                    am.Antrenor.Id,
                    am.Antrenor.AdSoyad,
                    am.Antrenor.UzmanlikAlani,
                    Salon = am.Antrenor.Salon.Ad,
                    Gun = am.Gun.ToString(),
                    BaslangicSaati = am.BaslangicSaati.ToString(@"hh\:mm"),
                    BitisSaati = am.BitisSaati.ToString(@"hh\:mm")
                })
                .ToListAsync();

            if (!antrenorler.Any())
            {
                return NotFound(new { message = $"{gun} günü müsait antrenör bulunamadı" });
            }

            return Ok(antrenorler);
        }

        // GET: api/AntrenorApi/BySalon/1
        // Belirli salondaki antrenörleri listele (LINQ)
        [HttpGet("BySalon/{salonId}")]
        public async Task<IActionResult> GetBySalon(int salonId)
        {
            var antrenorler = await _context.Antrenorler
                .Include(a => a.Salon)
                .Where(a => a.SalonId == salonId) // LINQ filtreleme
                .Select(a => new
                {
                    a.Id,
                    a.AdSoyad,
                    a.UzmanlikAlani,
                    Salon = a.Salon.Ad
                })
                .ToListAsync();

            if (!antrenorler.Any())
            {
                return NotFound(new { message = $"Salon ID {salonId} için antrenör bulunamadı" });
            }

            return Ok(antrenorler);
        }
    }
}

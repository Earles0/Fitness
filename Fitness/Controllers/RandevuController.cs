using Fitness.Data;
using Fitness.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RandevuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RandevuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Randevu
        public async Task<IActionResult> Index()
        {
            var randevular = await _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .OrderByDescending(r => r.TarihSaat)
                .ToListAsync();
            return View(randevular);
        }

        // GET: Randevu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                    .ThenInclude(a => a.Salon)
                .Include(r => r.Hizmet)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (randevu == null)
            {
                return NotFound();
            }

            return View(randevu);
        }

        // POST: Randevu/Onayla/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Onayla(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                randevu.OnaylandiMi = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu onaylandi!";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Randevu/Reddet/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reddet(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                randevu.OnaylandiMi = false;
                await _context.SaveChangesAsync();
                TempData["Warning"] = "Randevu reddedildi!";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Randevu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevular
                .Include(r => r.Uye)
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (randevu == null)
            {
                return NotFound();
            }

            return View(randevu);
        }

        // POST: Randevu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                _context.Randevular.Remove(randevu);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu silindi!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

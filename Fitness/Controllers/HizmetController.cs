using Fitness.Data;
using Fitness.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HizmetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HizmetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Hizmet
        public async Task<IActionResult> Index()
        {
            var hizmetler = await _context.Hizmetler
                .Include(h => h.Salon)
                .Include(h => h.AntrenorHizmetler)
                .ToListAsync();
            return View(hizmetler);
        }

        // GET: Hizmet/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hizmet = await _context.Hizmetler
                .Include(h => h.Salon)
                .Include(h => h.AntrenorHizmetler)
                    .ThenInclude(ah => ah.Antrenor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hizmet == null)
            {
                return NotFound();
            }

            return View(hizmet);
        }

        // GET: Hizmet/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Salonlar = new SelectList(await _context.Salonlar.ToListAsync(), "Id", "Ad");
            return View();
        }

        // POST: Hizmet/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ad,SureDakika,Ucret,SalonId")] Hizmet hizmet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hizmet);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Hizmet basariyla eklendi!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Salonlar = new SelectList(await _context.Salonlar.ToListAsync(), "Id", "Ad", hizmet.SalonId);
            return View(hizmet);
        }

        // GET: Hizmet/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hizmet = await _context.Hizmetler.FindAsync(id);
            if (hizmet == null)
            {
                return NotFound();
            }
            ViewBag.Salonlar = new SelectList(await _context.Salonlar.ToListAsync(), "Id", "Ad", hizmet.SalonId);
            return View(hizmet);
        }

        // POST: Hizmet/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,SureDakika,Ucret,SalonId")] Hizmet hizmet)
        {
            if (id != hizmet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hizmet);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Hizmet basariyla guncellendi!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HizmetExists(hizmet.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Salonlar = new SelectList(await _context.Salonlar.ToListAsync(), "Id", "Ad", hizmet.SalonId);
            return View(hizmet);
        }

        // GET: Hizmet/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hizmet = await _context.Hizmetler
                .Include(h => h.Salon)
                .Include(h => h.AntrenorHizmetler)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hizmet == null)
            {
                return NotFound();
            }

            return View(hizmet);
        }

        // POST: Hizmet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hizmet = await _context.Hizmetler
                .Include(h => h.AntrenorHizmetler)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hizmet != null)
            {
                // Iliskili kayitlari sil
                if (hizmet.AntrenorHizmetler.Any())
                {
                    _context.AntrenorHizmetler.RemoveRange(hizmet.AntrenorHizmetler);
                }

                _context.Hizmetler.Remove(hizmet);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Hizmet basariyla silindi!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool HizmetExists(int id)
        {
            return _context.Hizmetler.Any(e => e.Id == id);
        }
    }
}

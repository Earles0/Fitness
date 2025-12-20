using Fitness.Data;
using Fitness.Models;
using Fitness.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AntrenorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AntrenorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Antrenor
        public async Task<IActionResult> Index()
        {
            var antrenorler = await _context.Antrenorler
                .Include(a => a.Salon)
                .Include(a => a.AntrenorHizmetler)
                    .ThenInclude(ah => ah.Hizmet)
                .Include(a => a.Musaitlikler)
                .ToListAsync();
            return View(antrenorler);
        }

        // GET: Antrenor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var antrenor = await _context.Antrenorler
                .Include(a => a.Salon)
                .Include(a => a.AntrenorHizmetler)
                    .ThenInclude(ah => ah.Hizmet)
                .Include(a => a.Musaitlikler)
                .Include(a => a.Randevular)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (antrenor == null)
            {
                return NotFound();
            }

            return View(antrenor);
        }

        // GET: Antrenor/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new AntrenorCreateViewModel
            {
                Salonlar = new SelectList(await _context.Salonlar.ToListAsync(), "Id", "Ad"),
                AvailableHizmetler = await _context.Hizmetler.ToListAsync()
            };
            return View(viewModel);
        }

        // POST: Antrenor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AntrenorCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var antrenor = new Antrenor
                {
                    AdSoyad = viewModel.AdSoyad,
                    UzmanlikAlani = viewModel.UzmanlikAlani,
                    SalonId = viewModel.SalonId
                };

                _context.Add(antrenor);
                await _context.SaveChangesAsync();

                // Hizmet iliskilerini ekle
                if (viewModel.SelectedHizmetIds != null && viewModel.SelectedHizmetIds.Any())
                {
                    foreach (var hizmetId in viewModel.SelectedHizmetIds)
                    {
                        _context.AntrenorHizmetler.Add(new AntrenorHizmet
                        {
                            AntrenorId = antrenor.Id,
                            HizmetId = hizmetId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Antrenor basariyla eklendi!";
                return RedirectToAction(nameof(Index));
            }

            viewModel.Salonlar = new SelectList(await _context.Salonlar.ToListAsync(), "Id", "Ad", viewModel.SalonId);
            viewModel.AvailableHizmetler = await _context.Hizmetler.ToListAsync();
            return View(viewModel);
        }

        // GET: Antrenor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var antrenor = await _context.Antrenorler
                .Include(a => a.AntrenorHizmetler)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (antrenor == null)
            {
                return NotFound();
            }

            var viewModel = new AntrenorCreateViewModel
            {
                AdSoyad = antrenor.AdSoyad,
                UzmanlikAlani = antrenor.UzmanlikAlani,
                SalonId = antrenor.SalonId,
                SelectedHizmetIds = antrenor.AntrenorHizmetler.Select(ah => ah.HizmetId).ToList(),
                Salonlar = new SelectList(await _context.Salonlar.ToListAsync(), "Id", "Ad", antrenor.SalonId),
                AvailableHizmetler = await _context.Hizmetler.ToListAsync()
            };

            ViewBag.AntrenorId = id;
            return View(viewModel);
        }

        // POST: Antrenor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AntrenorCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var antrenor = await _context.Antrenorler
                        .Include(a => a.AntrenorHizmetler)
                        .FirstOrDefaultAsync(a => a.Id == id);

                    if (antrenor == null)
                    {
                        return NotFound();
                    }

                    antrenor.AdSoyad = viewModel.AdSoyad;
                    antrenor.UzmanlikAlani = viewModel.UzmanlikAlani;
                    antrenor.SalonId = viewModel.SalonId;

                    // Eski hizmetleri sil
                    _context.AntrenorHizmetler.RemoveRange(antrenor.AntrenorHizmetler);

                    // Yeni hizmetleri ekle
                    if (viewModel.SelectedHizmetIds != null && viewModel.SelectedHizmetIds.Any())
                    {
                        foreach (var hizmetId in viewModel.SelectedHizmetIds)
                        {
                            _context.AntrenorHizmetler.Add(new AntrenorHizmet
                            {
                                AntrenorId = antrenor.Id,
                                HizmetId = hizmetId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Antrenor basariyla guncellendi!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AntrenorExists(id))
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

            viewModel.Salonlar = new SelectList(await _context.Salonlar.ToListAsync(), "Id", "Ad", viewModel.SalonId);
            viewModel.AvailableHizmetler = await _context.Hizmetler.ToListAsync();
            ViewBag.AntrenorId = id;
            return View(viewModel);
        }

        // GET: Antrenor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var antrenor = await _context.Antrenorler
                .Include(a => a.Salon)
                .Include(a => a.AntrenorHizmetler)
                    .ThenInclude(ah => ah.Hizmet)
                .Include(a => a.Randevular)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (antrenor == null)
            {
                return NotFound();
            }

            return View(antrenor);
        }

        // POST: Antrenor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var antrenor = await _context.Antrenorler
                .Include(a => a.AntrenorHizmetler)  // İlişkileri yükle
                .Include(a => a.Musaitlikler)        // İlişkileri yükle
                .FirstOrDefaultAsync(a => a.Id == id);

            if (antrenor != null)
            {
                // Önce ilişkili AntrenorHizmet kayıtlarını sil
                if (antrenor.AntrenorHizmetler.Any())
                {
                    _context.AntrenorHizmetler.RemoveRange(antrenor.AntrenorHizmetler);
                }

                // Müsaitlik kayıtlarını sil
                if (antrenor.Musaitlikler.Any())
                {
                    _context.AntrenorMusaitlikler.RemoveRange(antrenor.Musaitlikler);
                }

                // Sonra Antrenörü sil
                _context.Antrenorler.Remove(antrenor);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Antrenor basariyla silindi!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AntrenorExists(int id)
        {
            return _context.Antrenorler.Any(e => e.Id == id);
        }
    }
}

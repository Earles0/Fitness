using Fitness.Data;
using Fitness.Models;
using Fitness.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Controllers
{
    [Authorize(Roles = "Uye")]
    public class UyeRandevuController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public UyeRandevuController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UyeRandevu
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var randevular = await _context.Randevular
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .Where(r => r.UyeId == userId)
                .OrderByDescending(r => r.TarihSaat)
                .ToListAsync();

            return View(randevular);
        }

        // GET: UyeRandevu/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new RandevuCreateViewModel
            {
                Hizmetler = new SelectList(await _context.Hizmetler.ToListAsync(), "Id", "Ad"),
                Antrenorler = new SelectList(await _context.Antrenorler.ToListAsync(), "Id", "AdSoyad"),
                TarihSaat = DateTime.Now.AddDays(1)
            };
            return View(viewModel);
        }

        // POST: UyeRandevu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RandevuCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);

                // Cakisma kontrolu
                var cakismaVarMi = await _context.Randevular
                    .AnyAsync(r => r.AntrenorId == viewModel.AntrenorId &&
                                   r.TarihSaat == viewModel.TarihSaat);

                if (cakismaVarMi)
                {
                    ModelState.AddModelError("", "Bu antrenor secilen saatte baska bir randevuya sahip!");
                }
                else
                {
                    var randevu = new Randevu
                    {
                        UyeId = userId!,
                        AntrenorId = viewModel.AntrenorId,
                        HizmetId = viewModel.HizmetId,
                        TarihSaat = viewModel.TarihSaat,
                        OnaylandiMi = false
                    };

                    _context.Randevular.Add(randevu);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Randevu talebi olusturuldu! Admin onayini bekliyor.";
                    return RedirectToAction(nameof(Index));
                }
            }

            viewModel.Hizmetler = new SelectList(await _context.Hizmetler.ToListAsync(), "Id", "Ad", viewModel.HizmetId);
            viewModel.Antrenorler = new SelectList(await _context.Antrenorler.ToListAsync(), "Id", "AdSoyad", viewModel.AntrenorId);
            return View(viewModel);
        }
    }
}

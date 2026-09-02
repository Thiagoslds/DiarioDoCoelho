using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiarioDoCoelho.Data;
using DiarioDoCoelho.Models;

namespace DiarioDoCoelho.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class BannersAfiliadosController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // GET: Admin/BannersAfiliados
        public async Task<IActionResult> Index()
        {
            return View(await _context.BannersAfiliados.ToListAsync());
        }

        // GET: Admin/BannersAfiliados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();

            var banner = await _context.BannersAfiliados.FirstOrDefaultAsync(b => b.Id == id);
            if (banner is null) return NotFound();

            return View(banner);
        }

        // GET: Admin/BannersAfiliados/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/BannersAfiliados/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,ImagemUrl,LinkAfiliado,Posicao,Ativo")] BannerAfiliado banner)
        {
            if (ModelState.IsValid)
            {
                _context.Add(banner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(banner);
        }

        // GET: Admin/BannersAfiliados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var banner = await _context.BannersAfiliados.FindAsync(id);
            if (banner is null) return NotFound();

            return View(banner);
        }

        // POST: Admin/BannersAfiliados/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,ImagemUrl,LinkAfiliado,Posicao,Ativo")] BannerAfiliado banner)
        {
            if (id != banner.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(banner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.BannersAfiliados.Any(b => b.Id == banner.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(banner);
        }

        // GET: Admin/BannersAfiliados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var banner = await _context.BannersAfiliados.FirstOrDefaultAsync(b => b.Id == id);
            if (banner is null) return NotFound();

            return View(banner);
        }

        // POST: Admin/BannersAfiliados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var banner = await _context.BannersAfiliados.FindAsync(id);
            if (banner is not null)
            {
                _context.BannersAfiliados.Remove(banner);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

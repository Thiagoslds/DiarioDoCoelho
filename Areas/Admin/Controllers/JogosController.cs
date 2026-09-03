using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiarioDoCoelho.Data;
using DiarioDoCoelho.Models;

namespace DiarioDoCoelho.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class JogosController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // GET: Admin/Jogos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Jogos.OrderByDescending(j => j.DataHora).ToListAsync());
        }

        // GET: Admin/Jogos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();

            var jogo = await _context.Jogos.FirstOrDefaultAsync(j => j.Id == id);
            if (jogo is null) return NotFound();

            return View(jogo);
        }

        // GET: Admin/Jogos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Jogos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Categoria,DataHora,Adversario,Campeonato,PlacarAmerica,PlacarAdversario,Mandante,LinkTabela")] Jogo jogo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jogo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jogo);
        }

        // GET: Admin/Jogos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo is null) return NotFound();

            return View(jogo);
        }

        // POST: Admin/Jogos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Categoria,DataHora,Adversario,Campeonato,PlacarAmerica,PlacarAdversario,Mandante,LinkTabela")] Jogo jogo)
        {
            if (id != jogo.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jogo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Jogos.Any(j => j.Id == jogo.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(jogo);
        }

        // GET: Admin/Jogos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var jogo = await _context.Jogos.FirstOrDefaultAsync(j => j.Id == id);
            if (jogo is null) return NotFound();

            return View(jogo);
        }

        // POST: Admin/Jogos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jogo = await _context.Jogos.FindAsync(id);
            if (jogo is not null)
            {
                _context.Jogos.Remove(jogo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

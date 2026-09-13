using DiarioDoCoelho.Data;
using DiarioDoCoelho.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiarioDoCoelho.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProdutosLojaController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // GET: Admin/ProdutosLoja
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProdutosLoja.ToListAsync());
        }

        // GET: Admin/ProdutosLoja/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ProdutosLoja/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,ImagemUrl,Descricao,Loja,LinkAfiliado,Ativo")] ProdutoLoja produtoLoja)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produtoLoja);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(produtoLoja);
        }

        // GET: Admin/ProdutosLoja/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produtoLoja = await _context.ProdutosLoja.FindAsync(id);
            if (produtoLoja == null)
            {
                return NotFound();
            }
            return View(produtoLoja);
        }

        // POST: Admin/ProdutosLoja/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,ImagemUrl,Descricao,Loja,LinkAfiliado,Ativo")] ProdutoLoja produtoLoja)
        {
            if (id != produtoLoja.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produtoLoja);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoLojaExists(produtoLoja.Id))
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
            return View(produtoLoja);
        }

        // GET: Admin/ProdutosLoja/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produtoLoja = await _context.ProdutosLoja
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produtoLoja == null)
            {
                return NotFound();
            }

            return View(produtoLoja);
        }

        // POST: Admin/ProdutosLoja/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produtoLoja = await _context.ProdutosLoja.FindAsync(id);
            if (produtoLoja != null)
            {
                _context.ProdutosLoja.Remove(produtoLoja);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoLojaExists(int id)
        {
            return _context.ProdutosLoja.Any(e => e.Id == id);
        }
    }
}

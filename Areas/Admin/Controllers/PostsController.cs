using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DiarioDoCoelho.Data;
using DiarioDoCoelho.Models;

namespace DiarioDoCoelho.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PostsController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // GET: Admin/Posts
        public async Task<IActionResult> Index()
        {
            var posts = _context.Posts.Include(p => p.Categoria).OrderByDescending(p => p.DataPublicacao);
            return View(await posts.ToListAsync());
        }

        // GET: Admin/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();

            var post = await _context.Posts.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);
            if (post is null) return NotFound();

            return View(post);
        }

        // GET: Admin/Posts/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias.OrderBy(c => c.Nome), "Id", "Nome");
            return View(new Post { DataPublicacao = DateTime.Now });
        }

        // POST: Admin/Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Resumo,Conteudo,DataPublicacao,Slug,ImagemCapa,CategoriaId,FonteNoticiaUrl,VideoEmbed,ProdutoAfiliadoUrl")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias.OrderBy(c => c.Nome), "Id", "Nome", post.CategoriaId);
            return View(post);
        }

        // GET: Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var post = await _context.Posts.FindAsync(id);
            if (post is null) return NotFound();

            ViewData["CategoriaId"] = new SelectList(_context.Categorias.OrderBy(c => c.Nome), "Id", "Nome", post.CategoriaId);
            return View(post);
        }

        // POST: Admin/Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Resumo,Conteudo,DataPublicacao,Slug,ImagemCapa,CategoriaId,FonteNoticiaUrl,VideoEmbed,ProdutoAfiliadoUrl")] Post post)
        {
            if (id != post.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Posts.Any(p => p.Id == post.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias.OrderBy(c => c.Nome), "Id", "Nome", post.CategoriaId);
            return View(post);
        }

        // GET: Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var post = await _context.Posts.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);
            if (post is null) return NotFound();

            return View(post);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post is not null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

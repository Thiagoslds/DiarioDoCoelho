using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiarioDoCoelho.Data;
using DiarioDoCoelho.Models;

namespace DiarioDoCoelho.Controllers;

public class PostController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    // GET: /Post
    public async Task<IActionResult> Index()
    {
        var posts = await _context.Posts
            .Include(p => p.Categoria)
            .Where(p => p.TipoPost == TipoPost.Noticia)
            .OrderByDescending(p => p.DataPublicacao)
            .ToListAsync();

        return View(posts);
    }

    // GET: /Post/Ler/manto-novo-do-america-e-lancado
    [Route("Post/Ler/{slug}")]
    public async Task<IActionResult> Ler(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return NotFound();
        }

        var post = await _context.Posts
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Slug == slug);

        if (post is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(post.FonteNoticiaUrl))
        {
            return RedirectToAction("Ler", "Giro", new { slug = post.Slug });
        }

        return View(post);
    }
}

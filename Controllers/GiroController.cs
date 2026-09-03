using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiarioDoCoelho.Data;

namespace DiarioDoCoelho.Controllers;

/// <summary>
/// Página intermediária ("Giro") exibida para notícias de parceiros externos,
/// com CTA para o site de origem e vitrine da Loja do Coelho.
/// </summary>
public class GiroController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    // GET: /Giro/titulo-da-noticia
    [Route("Giro/{slug}")]
    public async Task<IActionResult> Ler(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return NotFound();
        }

        var post = await _context.Posts
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Slug == slug);

        if (post is null || string.IsNullOrWhiteSpace(post.FonteNoticiaUrl))
        {
            return NotFound();
        }

        ViewBag.ProdutosLoja = ProdutosAfiliadosMock.Produtos.Take(4).ToList();

        return View(post);
    }
}

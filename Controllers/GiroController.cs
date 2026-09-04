using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiarioDoCoelho.Data;
using DiarioDoCoelho.ViewModels;

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

        var viewModel = new GiroLerViewModel
        {
            Titulo = post.Titulo,
            ImagemCapa = post.ImagemCapa,
            FonteNoticiaUrl = post.FonteNoticiaUrl,
            CategoriaNome = post.Categoria?.Nome,
            ProdutosLoja = ProdutosAfiliadosMock.Produtos.Take(4).ToList()
        };

        return View(viewModel);
    }

    // GET: /Giro/Externa?url=...&titulo=...&imagem=...&fonte=...
    [Route("Giro/Externa")]
    public IActionResult Externa(string url, string titulo, string? imagem, string? fonte)
    {
        if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(titulo))
        {
            return NotFound();
        }

        var viewModel = new GiroLerViewModel
        {
            Titulo = titulo,
            ImagemCapa = imagem,
            FonteNoticiaUrl = url,
            CategoriaNome = fonte,
            ProdutosLoja = ProdutosAfiliadosMock.Produtos.Take(4).ToList()
        };

        return View("Ler", viewModel);
    }
}


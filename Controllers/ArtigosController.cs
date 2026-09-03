using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiarioDoCoelho.Data;
using DiarioDoCoelho.Models;

namespace DiarioDoCoelho.Controllers;

/// <summary>
/// Listagem pública de artigos, análises e opiniões pessoais (Bloco 4 da Home).
/// </summary>
public class ArtigosController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    // GET: /Artigos
    public async Task<IActionResult> Index()
    {
        var artigos = await _context.Posts
            .Include(p => p.Categoria)
            .Where(p => p.TipoPost == TipoPost.Artigo)
            .OrderByDescending(p => p.DataPublicacao)
            .ToListAsync();

        return View(artigos);
    }
}

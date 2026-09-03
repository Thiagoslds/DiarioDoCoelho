using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiarioDoCoelho.Data;
using DiarioDoCoelho.Models;

namespace DiarioDoCoelho.Controllers;

/// <summary>
/// Listagem pública do "Baú do Coelho": conteúdo atemporal de nostalgia e história
/// do clube, com oportunidade de monetização via camisas retrô (Bloco 5 da Home).
/// </summary>
public class BauController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    // GET: /Bau
    public async Task<IActionResult> Index()
    {
        var historias = await _context.Posts
            .Include(p => p.Categoria)
            .Where(p => p.TipoPost == TipoPost.Historia)
            .OrderByDescending(p => p.DataPublicacao)
            .ToListAsync();

        return View(historias);
    }
}

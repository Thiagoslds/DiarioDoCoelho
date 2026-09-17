using Microsoft.AspNetCore.Mvc;
using DiarioDoCoelho.Data;
using DiarioDoCoelho.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DiarioDoCoelho.Controllers;

public class LojaController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    // GET: /Loja
    public async Task<IActionResult> Index()
    {
        var produtos = await _context.ProdutosLoja
            .Where(p => p.Ativo)
            .ToListAsync();
        return View(produtos);
    }
}

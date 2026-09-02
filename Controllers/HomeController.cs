using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiarioDoCoelho.Data;
using DiarioDoCoelho.Models;
using DiarioDoCoelho.ViewModels;

namespace DiarioDoCoelho.Controllers;

public class HomeController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IActionResult> Index()
    {
        var ultimosPosts = await _context.Posts
            .Include(p => p.Categoria)
            .OrderByDescending(p => p.DataPublicacao)
            .Take(10)
            .ToListAsync();

        var proximoJogo = await _context.Jogos
            .Where(j => j.DataHora >= DateTime.Now)
            .OrderBy(j => j.DataHora)
            .FirstOrDefaultAsync();

        var bannersAtivos = await _context.BannersAfiliados
            .Where(b => b.Ativo)
            .ToListAsync();

        var viewModel = new HomeIndexViewModel
        {
            UltimosPosts = ultimosPosts,
            ProximoJogo = proximoJogo,
            BannersAtivos = bannersAtivos
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

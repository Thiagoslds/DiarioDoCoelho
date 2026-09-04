using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiarioDoCoelho.Data;
using DiarioDoCoelho.Models;
using DiarioDoCoelho.ViewModels;

namespace DiarioDoCoelho.Controllers;

// Injeção de dependência via construtor primário (C# 12)
public class HomeController(ApplicationDbContext context, ExtratorNoticiasService extratorNoticias) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly ExtratorNoticiasService _extratorNoticias = extratorNoticias;

    public async Task<IActionResult> Index()
    {
        var ultimosPosts = await _context.Posts
            .Include(p => p.Categoria)
            .Where(p => p.TipoPost == TipoPost.Noticia)
            .OrderByDescending(p => p.DataPublicacao)
            .Take(11)
            .ToListAsync();

        var postDestaque = ultimosPosts.FirstOrDefault();
        var restantePosts = ultimosPosts.Skip(1).ToList();

        // Você pode manter o GiroNoticias do seu banco ou removê-lo se as notícias externas forem substituí-lo
        var giroNoticias = await _context.Posts
            .Include(p => p.Categoria)
            .Where(p => p.TipoPost == TipoPost.Noticia && p.FonteNoticiaUrl != null && p.FonteNoticiaUrl != "")
            .OrderByDescending(p => p.DataPublicacao)
            .Take(4)
            .ToListAsync();

        var artigos = await _context.Posts
            .Include(p => p.Categoria)
            .Where(p => p.TipoPost == TipoPost.Artigo)
            .OrderByDescending(p => p.DataPublicacao)
            .Take(2)
            .ToListAsync();

        var bauDoCoelho = await _context.Posts
            .Include(p => p.Categoria)
            .Where(p => p.TipoPost == TipoPost.Historia)
            .OrderByDescending(p => p.DataPublicacao)
            .Take(3)
            .ToListAsync();

        var jogosFuturos = await _context.Jogos
            .Where(j => j.DataHora >= DateTime.Now)
            .OrderBy(j => j.DataHora)
            .ToListAsync();

        var proximosJogos = jogosFuturos
            .GroupBy(j => j.Categoria)
            .Select(g => g.First())
            .OrderBy(j => j.Categoria)
            .ToList();

        var bannersAtivos = await _context.BannersAfiliados
            .Where(b => b.Ativo)
            .ToListAsync();

        // 1. Chama o serviço de raspagem (execução síncrona, pois HtmlAgilityPack trabalha bem assim em operações simples)
        var noticiasExternas = _extratorNoticias.ObterTodasNoticias();

        var viewModel = new HomeIndexViewModel
        {
            PostDestaque = postDestaque,
            UltimosPosts = restantePosts,
            ProximosJogos = proximosJogos,
            BannersAtivos = bannersAtivos,
            GiroNoticias = giroNoticias,
            Artigos = artigos,
            BauDoCoelho = bauDoCoelho,
            ProdutosLoja = ProdutosAfiliadosMock.Produtos,

            // 2. Passa as notícias raspadas para o ViewModel
            NoticiasExternas = noticiasExternas
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Termos()
    {
        return View();
    }

    public IActionResult Contato()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
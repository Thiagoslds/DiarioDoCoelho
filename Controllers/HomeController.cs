using DiarioDoCoelho.Data;
using DiarioDoCoelho.Models;
using DiarioDoCoelho.Services;
using DiarioDoCoelho.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DiarioDoCoelho.Controllers;

// Injeção de dependência via construtor primário (C# 12)
public class HomeController(ApplicationDbContext context, ExtratorNoticiasService extratorNoticias, ExtratorPartidasService extratorPartidas) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly ExtratorNoticiasService _extratorNoticias = extratorNoticias;
    private readonly ExtratorPartidasService _extratorPartidas = extratorPartidas;

    public class PartidaContainer
    {
        public PartidaViewModel? Anterior { get; set; }
        public PartidaViewModel? Proximo { get; set; }
        public List<PartidaViewModel> ProximosJogos { get; set; } = new();
        public List<PartidaViewModel> JogosAnteriores { get; set; } = new();
    }

    public async Task<IActionResult> Index()
    {
        // Artigos (antigas "Notícias Manuais")
        var ultimosPosts = await _context.Posts
            .Include(p => p.Categoria)
            .Where(p => p.TipoPost == TipoPost.Artigo)
            .OrderByDescending(p => p.DataPublicacao)
            .Take(11)
            .ToListAsync();

        var postDestaque = ultimosPosts.FirstOrDefault();
        var restantePosts = ultimosPosts.Skip(1).ToList();

        var artigos = ultimosPosts.Take(2).ToList(); // Podemos usar os mesmos para 'artigos' se necessário, ou deixar vazio se o layout mudou.

        // Leitura do banco de dados (JSON) para dados extraídos
        var dbNoticias = await _context.DadosExtraidos.FirstOrDefaultAsync(d => d.Chave == "Noticias");
        var noticiasExternas = dbNoticias != null ? System.Text.Json.JsonSerializer.Deserialize<List<NoticiaCoelho>>(dbNoticias.ConteudoJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new() : new();

        var dbPartidas = await _context.DadosExtraidos.FirstOrDefaultAsync(d => d.Chave == "Partidas");
        var jogosAnteriorProximo = dbPartidas != null ? System.Text.Json.JsonSerializer.Deserialize<PartidaContainer>(dbPartidas.ConteudoJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) : null;

        var bannersAtivos = await _context.BannersAfiliados
            .Where(b => b.Ativo)
            .ToListAsync();

        var produtosLoja = await _context.ProdutosLoja
            .Where(p => p.Ativo)
            .ToListAsync();

        var viewModel = new HomeIndexViewModel
        {
            PostDestaque = postDestaque,
            UltimosPosts = restantePosts,
            PartidaAnterior = jogosAnteriorProximo?.Anterior,
            ProximaPartida = jogosAnteriorProximo?.Proximo,
            BannersAtivos = bannersAtivos,
            GiroNoticias = new List<Post>(), // Removido
            Artigos = artigos,
            ProdutosLoja = produtosLoja,

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
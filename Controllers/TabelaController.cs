using DiarioDoCoelho.Services;
using DiarioDoCoelho.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiarioDoCoelho.Controllers
{
    public class TabelaController(DiarioDoCoelho.Data.ApplicationDbContext context) : Controller
    {
        private readonly DiarioDoCoelho.Data.ApplicationDbContext _context = context;

        public async Task<IActionResult> Index()
        {
            var dbClassificacao = await _context.DadosExtraidos.FirstOrDefaultAsync(d => d.Chave == "ClassificacaoGeral");
            var classificacao = dbClassificacao != null ? System.Text.Json.JsonSerializer.Deserialize<List<ClassificacaoViewModel>>(dbClassificacao.ConteudoJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new() : new();

            var dbPartidas = await _context.DadosExtraidos.FirstOrDefaultAsync(d => d.Chave == "Partidas");
            var partidas = dbPartidas != null ? System.Text.Json.JsonSerializer.Deserialize<DiarioDoCoelho.Controllers.HomeController.PartidaContainer>(dbPartidas.ConteudoJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) : null;

            var viewModel = new TabelaIndexViewModel
            {
                Classificacao = classificacao,
                ProximosJogos = partidas?.ProximosJogos ?? new(),
                JogosAnteriores = partidas?.JogosAnteriores ?? new()
            };

            return View(viewModel);
        }
    }
}
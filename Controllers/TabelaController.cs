using DiarioDoCoelho.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiarioDoCoelho.Controllers
{
    public class TabelaController : Controller
    {
        private readonly TabelaCampeonatoService _tabelaService;

        public TabelaController(TabelaCampeonatoService tabelaService)
        {
            _tabelaService = tabelaService;
        }

        public async Task<IActionResult> Index()
        {
            // ID 10 geralmente refere-se ao Brasileirão Série A na API-Futebol.
            // Troque caso o América-MG esteja disputando outro campeonato (ex: Série B ou Mineiro).
            var tabela = await _tabelaService.ObterTabelaAsync(10);
            return View(tabela);
        }
    }
}
using DiarioDoCoelho.Services;
using DiarioDoCoelho.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DiarioDoCoelho.Controllers
{
    public class TabelaController : Controller
    {
        private readonly ExtratorClassificacaoService _extratorClassificacao;
        private readonly ExtratorPartidasService _extratorPartidas;

        public TabelaController(ExtratorClassificacaoService extratorClassificacao, ExtratorPartidasService extratorPartidas)
        {
            _extratorClassificacao = extratorClassificacao;
            _extratorPartidas = extratorPartidas;
        }

        public IActionResult Index()
        {
            var classificacao = _extratorClassificacao.ObterClassificacao();

            // Recebe os 4 itens da Tupla atualizada
            var (anterior, proximo, proximosJogos, jogosAnteriores) = _extratorPartidas.ObterJogosAmerica();

            var viewModel = new TabelaIndexViewModel
            {
                Classificacao = classificacao,
                ProximosJogos = proximosJogos,
                JogosAnteriores = jogosAnteriores
            };

            return View(viewModel);
        }
    }
}
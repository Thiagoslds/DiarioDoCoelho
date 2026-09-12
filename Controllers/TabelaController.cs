using DiarioDoCoelho.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiarioDoCoelho.Controllers
{
    public class TabelaController : Controller
    {
        private readonly ExtratorClassificacaoService _extratorClassificacao;

        public TabelaController(ExtratorClassificacaoService extratorClassificacao)
        {
            _extratorClassificacao = extratorClassificacao;
        }

        public IActionResult Index()
        {
            // Busca a tabela atualizada direto da CBF usando o novo serviço
            var tabela = _extratorClassificacao.ObterClassificacao();

            return View(tabela);
        }
    }
}
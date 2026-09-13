using DiarioDoCoelho.Services;
using DiarioDoCoelho.ViewModels;
using Microsoft.AspNetCore.Mvc;

public class DnaFormadorController(ExtratorSub20Service extratorSub20) : Controller
{
    private readonly ExtratorSub20Service _extratorSub20 = extratorSub20;

    public IActionResult Index()
    {
        var jogos = _extratorSub20.ObterJogosSub20();

        var viewModel = new DnaFormadorViewModel
        {
            PartidaAnterior = jogos.Anterior,
            ProximaPartida = jogos.Proximo,
            Classificacao = _extratorSub20.ObterClassificacaoSub20()
        };

        return View(viewModel);
    }
}
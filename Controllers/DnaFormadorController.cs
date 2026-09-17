using DiarioDoCoelho.Data;
using DiarioDoCoelho.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class DnaFormadorController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IActionResult> Index()
    {
        // 1. Obter os jogos (agora armazenados na chave Partidas, caso queira compartilhar, mas o usuário comentou o Sub20. Vamos deixar nulo por enquanto)
        // Se no futuro você colocar os jogos Sub20 no banco, poderá ler da mesma forma.

        var viewModel = new DnaFormadorViewModel
        {
            PartidaAnterior = null,
            ProximaPartida = null
        };

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // 2. Tabela Mineiro Sub-20
        var dbSub20 = await _context.DadosExtraidos.FirstOrDefaultAsync(d => d.Chave == "ClassificacaoSub20");
        if (dbSub20 != null && !string.IsNullOrEmpty(dbSub20.ConteudoJson))
        {
            viewModel.ClassificacaoMineiroSub20 = JsonSerializer.Deserialize<List<ClassificacaoViewModel>>(dbSub20.ConteudoJson, options) ?? new();
        }

        // 3. Tabela Mineiro Sub-17
        var dbMineiroSub17 = await _context.DadosExtraidos.FirstOrDefaultAsync(d => d.Chave == "ClassificacaoMineiroSub17");
        if (dbMineiroSub17 != null && !string.IsNullOrEmpty(dbMineiroSub17.ConteudoJson))
        {
            viewModel.ClassificacaoMineiroSub17 = JsonSerializer.Deserialize<List<ClassificacaoViewModel>>(dbMineiroSub17.ConteudoJson, options) ?? new();
        }

        // 4. Tabela Brasileiro Sub-17
        var dbBrasileiroSub17 = await _context.DadosExtraidos.FirstOrDefaultAsync(d => d.Chave == "ClassificacaoBrasileiroSub17");
        if (dbBrasileiroSub17 != null && !string.IsNullOrEmpty(dbBrasileiroSub17.ConteudoJson))
        {
            viewModel.ClassificacaoBrasileiroSub17 = JsonSerializer.Deserialize<List<ClassificacaoViewModel>>(dbBrasileiroSub17.ConteudoJson, options) ?? new();
        }

        return View(viewModel);
    }
}
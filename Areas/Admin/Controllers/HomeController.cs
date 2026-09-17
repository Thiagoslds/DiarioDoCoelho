using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiarioDoCoelho.Data;
using DiarioDoCoelho.ViewModels;

namespace DiarioDoCoelho.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // GET: Admin/Home
        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalPosts = await _context.Posts.Where(p => p.TipoPost == DiarioDoCoelho.Models.TipoPost.Artigo).CountAsync(),
                TotalCategorias = await _context.Categorias.CountAsync(),
                TotalProdutos = await _context.ProdutosLoja.CountAsync(),
                TotalBanners = await _context.BannersAfiliados.CountAsync(),
                UltimosPosts = await _context.Posts
                    .Include(p => p.Categoria)
                    .Where(p => p.TipoPost == DiarioDoCoelho.Models.TipoPost.Artigo)
                    .OrderByDescending(p => p.DataPublicacao)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarExtratores(
            [FromServices] ExtratorNoticiasService extratorNoticias,
            [FromServices] DiarioDoCoelho.Services.ExtratorPartidasService extratorPartidas,
            [FromServices] DiarioDoCoelho.Services.ExtratorClassificacaoService extratorClassificacao,
            [FromServices] DiarioDoCoelho.Services.ExtratorDnaFormadorService extratorDnaFormador)
        {
            var mensagensErro = new List<string>();
            var dadosParaSalvar = new Dictionary<string, object>();

            try
            {
                var noticias = extratorNoticias.ObterTodasNoticias();
                dadosParaSalvar.Add("Noticias", noticias);
            }
            catch (Exception ex) { mensagensErro.Add("Notícias: " + ex.Message); }

            try
            {
                var partidas = extratorPartidas.ObterJogosAmerica();
                dadosParaSalvar.Add("Partidas", new { 
                    Anterior = partidas.Anterior, 
                    Proximo = partidas.Proximo, 
                    ProximosJogos = partidas.ProximosJogos, 
                    JogosAnteriores = partidas.JogosAnteriores 
                });
            }
            catch (Exception ex) { mensagensErro.Add("Partidas: " + ex.Message); }

            try
            {
                var classificacaoGeral = extratorClassificacao.ObterClassificacao();
                dadosParaSalvar.Add("ClassificacaoGeral", classificacaoGeral);
            }
            catch (Exception ex) { mensagensErro.Add("Classificação Geral: " + ex.Message); }

            try
            {
                var classificacaoSub20 = extratorDnaFormador.ObterClassificacaoSub20();
                dadosParaSalvar.Add("ClassificacaoSub20", classificacaoSub20);
            }
            catch (Exception ex) { mensagensErro.Add("Classificação Mineiro Sub-20: " + ex.Message); }

            try
            {
                var classificacaoMineiroSub17 = extratorDnaFormador.ObterClassificacaoMineiroSub17();
                dadosParaSalvar.Add("ClassificacaoMineiroSub17", classificacaoMineiroSub17);
            }
            catch (Exception ex) { mensagensErro.Add("Classificação Mineiro Sub-17: " + ex.Message); }

            try
            {
                var classificacaoBrasileiroSub17 = extratorDnaFormador.ObterClassificacaoBrasileiroSub17();
                dadosParaSalvar.Add("ClassificacaoBrasileiroSub17", classificacaoBrasileiroSub17);
            }
            catch (Exception ex) { mensagensErro.Add("Classificação Brasileiro Sub-17: " + ex.Message); }

            try
            {
                foreach (var (chave, objeto) in dadosParaSalvar)
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(objeto);
                    var registro = await _context.DadosExtraidos.FirstOrDefaultAsync(d => d.Chave == chave);

                    if (registro == null)
                    {
                        _context.DadosExtraidos.Add(new DiarioDoCoelho.Models.DadoExtraido
                        {
                            Chave = chave,
                            ConteudoJson = json,
                            DataAtualizacao = DateTime.Now
                        });
                    }
                    else
                    {
                        registro.ConteudoJson = json;
                        registro.DataAtualizacao = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();

                if (mensagensErro.Any())
                {
                    TempData["MensagemErro"] = "Atualização parcial. Erros: " + string.Join(" | ", mensagensErro);
                }
                else
                {
                    TempData["MensagemSucesso"] = "Dados dos extratores atualizados com sucesso!";
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro fatal ao salvar extratores: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

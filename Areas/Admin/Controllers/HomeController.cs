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
            [FromServices] DiarioDoCoelho.Services.ExtratorSub20Service extratorSub20)
        {
            try
            {
                // Extração
                var noticias = extratorNoticias.ObterTodasNoticias();
                var partidas = extratorPartidas.ObterJogosAmerica();
                var classificacaoGeral = extratorClassificacao.ObterClassificacao();
                var classificacaoSub20 = extratorSub20.ObterClassificacaoSub20();

                // Dicionário com Chave -> Objeto extraído
                var dadosParaSalvar = new Dictionary<string, object>
                {
                    { "Noticias", noticias },
                    { "Partidas", new { 
                        Anterior = partidas.Anterior, 
                        Proximo = partidas.Proximo, 
                        ProximosJogos = partidas.ProximosJogos, 
                        JogosAnteriores = partidas.JogosAnteriores 
                    } },
                    { "ClassificacaoGeral", classificacaoGeral },
                    { "ClassificacaoSub20", classificacaoSub20 }
                };

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
                TempData["MensagemSucesso"] = "Dados dos extratores atualizados com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro ao atualizar extratores: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

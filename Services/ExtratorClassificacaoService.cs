using HtmlAgilityPack;
using DiarioDoCoelho.ViewModels;

namespace DiarioDoCoelho.Services
{
    public class ExtratorClassificacaoService
    {
        private readonly HtmlWeb _web;

        public ExtratorClassificacaoService()
        {
            _web = new HtmlWeb
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36"
            };
        }

        public List<ClassificacaoViewModel> ObterClassificacao()
        {
            var classificacao = new List<ClassificacaoViewModel>();

            try
            {
                // URL oficial de classificação que reflete a estrutura da CBF
                var url = "https://www.cbf.com.br/futebol-brasileiro/tabelas/campeonato-brasileiro/serie-b/2026";
                var document = _web.Load(url);

                // Localiza as linhas da tabela dentro do corpo principal
                var rows = document.DocumentNode.SelectNodes("//table[contains(@class, 'styles_table')]/tbody/tr");

                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        var tds = row.SelectNodes("td");
                        if (tds == null || tds.Count < 13) continue;

                        var posNode = tds[0].SelectSingleNode(".//strong[contains(@class, 'styles_position')]");
                        var nomeNode = tds[0].SelectSingleNode(".//strong[contains(@class, 'styles_teamName')]");
                        var linkNode = tds[0].SelectSingleNode(".//a");

                        int timeId = 0;
                        string escudoUrl = "";

                        if (linkNode != null)
                        {
                            var href = linkNode.GetAttributeValue("href", "");
                            var partesUrl = href.Split('/');
                            if (partesUrl.Length > 0 && int.TryParse(partesUrl.Last(), out int extractedId))
                            {
                                timeId = extractedId;
                                escudoUrl = $"https://conteudo.cbf.com.br/clubes/{timeId}/escudo.jpg";
                            }
                        }

                        var ultimosJogos = new List<string>();
                        var svgNodes = tds[12].SelectNodes(".//svg/circle");
                        if (svgNodes != null)
                        {
                            foreach (var circle in svgNodes)
                            {
                                var fill = circle.GetAttributeValue("fill", "").ToUpper();
                                if (fill == "#24C796") ultimosJogos.Add("v");
                                else if (fill == "#B7B7B7") ultimosJogos.Add("e");
                                else if (fill == "#EE2D44") ultimosJogos.Add("d");
                            }
                        }

                        var item = new ClassificacaoViewModel
                        {
                            Posicao = int.TryParse(posNode?.InnerText.Trim(), out int pos) ? pos : 0,
                            Pontos = int.TryParse(tds[1].InnerText.Trim(), out int pts) ? pts : 0,
                            Jogos = int.TryParse(tds[2].InnerText.Trim(), out int j) ? j : 0,
                            Vitorias = int.TryParse(tds[3].InnerText.Trim(), out int v) ? v : 0,
                            Empates = int.TryParse(tds[4].InnerText.Trim(), out int e) ? e : 0,
                            Derrotas = int.TryParse(tds[5].InnerText.Trim(), out int d) ? d : 0,
                            GolsPro = int.TryParse(tds[6].InnerText.Trim(), out int gp) ? gp : 0,
                            GolsContra = int.TryParse(tds[7].InnerText.Trim(), out int gc) ? gc : 0,
                            SaldoGols = int.TryParse(tds[8].InnerText.Trim(), out int sg) ? sg : 0,
                            Aproveitamento = double.TryParse(tds[11].InnerText.Trim(), out double apv) ? apv : 0,
                            UltimosJogos = ultimosJogos,
                            Time = new TimeInfo
                            {
                                TimeId = timeId,
                                NomePopular = nomeNode?.InnerText.Trim() ?? "Desconhecido",
                                Escudo = escudoUrl
                            }
                        };

                        classificacao.Add(item);
                    }
                }
                else
                {
                    var title = document.DocumentNode.SelectSingleNode("//title")?.InnerText;
                    throw new Exception($"Tabela não encontrada. A página pode ter bloqueado o acesso. Título da página retornada: {title}");
                }
                
                if (!classificacao.Any())
                {
                    throw new Exception("A tabela foi encontrada, mas nenhum dado foi extraído. Possível mudança na estrutura da página.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao extrair classificação: {ex.Message}");
                throw; // Repassa a exceção para o Admin HomeController capturar
            }

            return classificacao;
        }
    }
}
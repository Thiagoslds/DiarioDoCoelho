using HtmlAgilityPack;
using DiarioDoCoelho.ViewModels;

namespace DiarioDoCoelho.Services
{
    public class ExtratorClassificacaoService
    {
        private readonly HtmlWeb _web;

        public ExtratorClassificacaoService()
        {
            _web = new HtmlWeb();
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

                        // 1. Posição, Nome e ID do Time (Primeira coluna)
                        var posNode = tds[0].SelectSingleNode(".//strong[contains(@class, 'styles_position')]");
                        var nomeNode = tds[0].SelectSingleNode(".//strong[contains(@class, 'styles_teamName')]");
                        var linkNode = tds[0].SelectSingleNode(".//a");

                        int timeId = 0;
                        string escudoUrl = "";

                        if (linkNode != null)
                        {
                            var href = linkNode.GetAttributeValue("href", "");
                            var partesUrl = href.Split('/');

                            // O ID do time fica sempre no final da URL da CBF
                            if (partesUrl.Length > 0 && int.TryParse(partesUrl.Last(), out int extractedId))
                            {
                                timeId = extractedId;
                                // Reconstrói a URL limpa do escudo usando o servidor de conteúdo da CBF
                                escudoUrl = $"https://conteudo.cbf.com.br/clubes/{timeId}/escudo.jpg";
                            }
                        }

                        // 2. Últimos Jogos (Última coluna de dados antes de "Próximo")
                        var ultimosJogos = new List<string>();
                        var svgNodes = tds[12].SelectNodes(".//svg/circle");

                        if (svgNodes != null)
                        {
                            foreach (var circle in svgNodes)
                            {
                                var fill = circle.GetAttributeValue("fill", "").ToUpper();

                                // Mapeamento das cores da CBF para as letras da View
                                if (fill == "#24C796") ultimosJogos.Add("v");
                                else if (fill == "#B7B7B7") ultimosJogos.Add("e");
                                else if (fill == "#EE2D44") ultimosJogos.Add("d");
                            }
                        }

                        // 3. Montagem do ViewModel
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
                            // As colunas 9 (CA) e 10 (CV) da CBF são ignoradas; o aproveitamento está na coluna 11
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao extrair classificação: {ex.Message}");
            }

            return classificacao;
        }
    }
}
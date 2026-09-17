using HtmlAgilityPack;
using DiarioDoCoelho.ViewModels;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DiarioDoCoelho.Services
{
    public class ExtratorDnaFormadorService
    {
        private readonly HtmlWeb _web;

        public ExtratorDnaFormadorService()
        {
            _web = new HtmlWeb
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36"
            };
        }

        // 1. JOGOS (DEIXADO VAZIO PARA IMPLEMENTAÇÃO FUTURA)
        public (PartidaViewModel? Anterior, PartidaViewModel? Proximo) ObterJogosSub20()
        {
            return (null, null);
        }

        // 2. CLASSIFICAÇÃO: MINEIRO SUB-20 (FMF)
        public List<ClassificacaoViewModel> ObterClassificacaoSub20()
        {
            var classificacao = new List<ClassificacaoViewModel>();

            try
            {
                string urlFmf = "https://fmf.com.br/Competicoes/ProxJogos.aspx?d=6";
                var document = _web.Load(urlFmf);

                var tableRows = document.DocumentNode.SelectNodes("//div[@id='cfase_1-3']//table//tr");

                if (tableRows != null)
                {
                    foreach (var tr in tableRows)
                    {
                        var tds = tr.SelectNodes("td");

                        if (tds == null || tds.Count < 12) continue;

                        string posText = tds[0].InnerText.Trim();
                        if (!int.TryParse(posText, out int posicao)) continue;

                        string nomeTime = FormatarNomeTime(tds[2].InnerText.Trim());
                        string escudoUrl = tds[1].SelectSingleNode(".//img")?.GetAttributeValue("src", "") ?? "";

                        classificacao.Add(new ClassificacaoViewModel
                        {
                            Posicao = posicao,
                            Time = new TimeInfo { NomePopular = nomeTime, Escudo = string.IsNullOrEmpty(escudoUrl) ? GerarFallbackAvatar(nomeTime) : escudoUrl },
                            Pontos = int.TryParse(tds[3].InnerText.Trim(), out int pts) ? pts : 0,
                            Jogos = int.TryParse(tds[4].InnerText.Trim(), out int j) ? j : 0,
                            Vitorias = int.TryParse(tds[5].InnerText.Trim(), out int v) ? v : 0,
                            Empates = int.TryParse(tds[6].InnerText.Trim(), out int e) ? e : 0,
                            Derrotas = int.TryParse(tds[7].InnerText.Trim(), out int d) ? d : 0,
                            GolsPro = int.TryParse(tds[8].InnerText.Trim(), out int gp) ? gp : 0,
                            GolsContra = int.TryParse(tds[9].InnerText.Trim(), out int gc) ? gc : 0,
                            SaldoGols = int.TryParse(tds[10].InnerText.Trim(), out int sg) ? sg : 0,
                            Aproveitamento = int.TryParse(tds[11].InnerText.Replace("%", "").Trim(), out int apr) ? apr : 0
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao extrair classificação sub-20 (FMF): {ex.Message}");
            }

            return classificacao;
        }

        // 3. CLASSIFICAÇÃO: MINEIRO SUB-17 (FMF)
        public List<ClassificacaoViewModel> ObterClassificacaoMineiroSub17()
        {
            var classificacao = new List<ClassificacaoViewModel>();

            try
            {
                string urlFmf = "https://fmf.com.br/Competicoes/ProxJogos.aspx?d=5";
                var document = _web.Load(urlFmf);

                var tableRows = document.DocumentNode.SelectNodes("//div[@id='cfase_1-2']//table//tr");

                if (tableRows != null)
                {
                    foreach (var tr in tableRows)
                    {
                        var tds = tr.SelectNodes("td");

                        if (tds == null || tds.Count < 12) continue;

                        string posText = tds[0].InnerText.Trim();
                        if (!int.TryParse(posText, out int posicao)) continue;

                        string nomeTime = FormatarNomeTime(tds[2].InnerText.Trim());
                        string escudoUrl = tds[1].SelectSingleNode(".//img")?.GetAttributeValue("src", "") ?? "";

                        classificacao.Add(new ClassificacaoViewModel
                        {
                            Posicao = posicao,
                            Time = new TimeInfo { NomePopular = nomeTime, Escudo = string.IsNullOrEmpty(escudoUrl) ? GerarFallbackAvatar(nomeTime) : escudoUrl },
                            Pontos = int.TryParse(tds[3].InnerText.Trim(), out int pts) ? pts : 0,
                            Jogos = int.TryParse(tds[4].InnerText.Trim(), out int j) ? j : 0,
                            Vitorias = int.TryParse(tds[5].InnerText.Trim(), out int v) ? v : 0,
                            Empates = int.TryParse(tds[6].InnerText.Trim(), out int e) ? e : 0,
                            Derrotas = int.TryParse(tds[7].InnerText.Trim(), out int d) ? d : 0,
                            GolsPro = int.TryParse(tds[8].InnerText.Trim(), out int gp) ? gp : 0,
                            GolsContra = int.TryParse(tds[9].InnerText.Trim(), out int gc) ? gc : 0,
                            SaldoGols = int.TryParse(tds[10].InnerText.Trim(), out int sg) ? sg : 0,
                            Aproveitamento = int.TryParse(tds[11].InnerText.Replace("%", "").Trim(), out int apr) ? apr : 0
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao extrair classificação Mineiro Sub-17 (FMF): {ex.Message}");
            }

            return classificacao;
        }

        // 4. CLASSIFICAÇÃO: BRASILEIRO SUB-17 (CBF) - Ajustado para o novo HTML
        public List<ClassificacaoViewModel> ObterClassificacaoBrasileiroSub17()
        {
            var classificacao = new List<ClassificacaoViewModel>();

            try
            {
                string urlCbf = "https://www.cbf.com.br/futebol-brasileiro/tabelas/campeonato-brasileiro/sub-17/2026";
                var document = _web.Load(urlCbf);

                // Mapeia as linhas do corpo da tabela conforme a estrutura moderna da CBF
                var tableRows = document.DocumentNode.SelectNodes("//tbody[contains(@class, 'styles_tbody__nCyL0')]//tr");

                if (tableRows != null)
                {
                    foreach (var tr in tableRows)
                    {
                        var tds = tr.SelectNodes("td");
                        if (tds == null || tds.Count < 12) continue;

                        int ParseIntSafe(string text) => int.TryParse(Regex.Match(text ?? "", @"-?\d+").Value, out int result) ? result : 0;

                        // Posição está dentro da tag forte do primeiro bloco
                        string posText = tds[0].SelectSingleNode(".//strong[contains(@class, 'styles_position__EF_xk')]")?.InnerText ?? "0";
                        int posicao = ParseIntSafe(posText);
                        if (posicao == 0) continue;

                        // Nome do time e escudo
                        string nomeCru = tds[0].SelectSingleNode(".//strong[contains(@class, 'styles_teamName__i7YUy')]")?.InnerText ?? "";
                        string nomeTime = FormatarNomeTime(nomeCru.Trim());

                        string escudoUrl = tds[0].SelectSingleNode(".//img")?.GetAttributeValue("src", "") ?? "";

                        // NOVO: Desempacota a URL real escondida no otimizador do Next.js
                        var matchUrl = Regex.Match(escudoUrl, @"url=([^&]+)");
                        if (matchUrl.Success)
                        {
                            // Transforma "https%3A%2F%2F..." em "https://..."
                            escudoUrl = System.Net.WebUtility.UrlDecode(matchUrl.Groups[1].Value);
                        }
                        else if (escudoUrl.StartsWith("/"))
                        {
                            escudoUrl = "https://www.cbf.com.br" + escudoUrl;
                        }

                        // Sequência das colunas na tabela da CBF:
                        // [0] Time, [1] PTS, [2] J, [3] V, [4] E, [5] D, [6] GP, [7] GC, [8] SG, [9] CA, [10] CV, [11] %
                        classificacao.Add(new ClassificacaoViewModel
                        {
                            Posicao = posicao,
                            Time = new TimeInfo { NomePopular = nomeTime, Escudo = string.IsNullOrEmpty(escudoUrl) ? GerarFallbackAvatar(nomeTime) : escudoUrl },
                            Pontos = ParseIntSafe(tds[1].InnerText),
                            Jogos = ParseIntSafe(tds[2].InnerText),
                            Vitorias = ParseIntSafe(tds[3].InnerText),
                            Empates = ParseIntSafe(tds[4].InnerText),
                            Derrotas = ParseIntSafe(tds[5].InnerText),
                            GolsPro = ParseIntSafe(tds[6].InnerText),
                            GolsContra = ParseIntSafe(tds[7].InnerText),
                            SaldoGols = ParseIntSafe(tds[8].InnerText),
                            Aproveitamento = ParseIntSafe(tds[11].InnerText)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao extrair classificação Brasileiro Sub-17 (CBF): {ex.Message}");
            }

            return classificacao;
        }

        // =========================================================
        // MÉTODOS AUXILIARES
        // =========================================================

        private string FormatarNomeTime(string nomeCru)
        {
            string nome = nomeCru.ToUpper();

            if (nome.Contains("AMÉRICA") || nome == "AMÉRICA" || nome == "AMERICA") return "América";
            if (nome.Contains("CRUZEIRO")) return "Cruzeiro";
            if (nome.Contains("ATLÉTICO") || nome.Contains("ATLETICO")) return "Atlético";

            var textInfo = new CultureInfo("pt-BR", false).TextInfo;
            return textInfo.ToTitleCase(nomeCru.ToLower());
        }

        private string ObterCaminhoEscudo(string nomeCru)
        {
            var nomeNormalizado = nomeCru.ToLower()
                .Replace("á", "a").Replace("ã", "a").Replace("é", "e")
                .Replace("í", "i").Replace("ó", "o").Replace("ô", "o")
                .Replace("ç", "c").Trim();

            if (nomeNormalizado.Contains("america")) return "/img/afc-escudos-site-branco-1.png";
            if (nomeNormalizado.Contains("cruzeiro")) return "/img/cruzeiro_1.png";

            if (nomeNormalizado == "atletico" || nomeNormalizado.Contains("atletico-mg") || nomeNormalizado.Contains("atletico mineiro") || nomeNormalizado.Contains("atlético saf"))
                return "/img/brazil_atletico-mineiro_700x700.football-logos.cc.png";

            var mapaTimes = new Dictionary<string, string>
            {
                { "athletic", "athletic" },
                { "atletico-go", "atletico-goianiense" },
                { "atletico goianiense", "atletico-goianiense" },
                { "itabirito", "itabirito" },
                { "coimbra", "coimbra" },
                { "nacional", "nacional-muriae" },
                { "betim", "betim" },
                { "boston", "boston-city" },
                { "sao joao", "sao-joao-del-rei" }
            };

            var chaveEncontrada = mapaTimes.Keys.FirstOrDefault(k => nomeNormalizado.Contains(k));
            if (chaveEncontrada != null)
            {
                return $"/img/brazil_{mapaTimes[chaveEncontrada]}_700x700.football-logos.cc.png";
            }

            return "/img/escudo-ausente.png";
        }

        private string GerarFallbackAvatar(string nomeTime)
        {
            if (nomeTime == "América") return "/img/afc-escudos-site-branco-1.png";
            return $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(nomeTime)}&background=e6f2e6&color=009e4f&rounded=true&bold=true";
        }
    }
}
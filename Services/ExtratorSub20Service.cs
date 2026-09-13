using HtmlAgilityPack;
using DiarioDoCoelho.ViewModels;
using System.Text.RegularExpressions;

namespace DiarioDoCoelho.Services
{
    public class ExtratorSub20Service
    {
        private readonly HtmlWeb _web;

        public ExtratorSub20Service()
        {
            _web = new HtmlWeb();
        }


        public (PartidaViewModel? Anterior, PartidaViewModel? Proximo) ObterJogosSub20()
        {
            var passados = new List<PartidaViewModel>();
            var futuros = new List<PartidaViewModel>();

            try
            {
                var document = _web.Load("https://www.ogol.com.br/equipe/america-mineiro/26870/todos-os-jogos");

                // Pega as linhas dos jogos na tabela principal[cite: 10]
                var rows = document.DocumentNode.SelectNodes("//table[contains(@class, 'zztable stats')]/tbody/tr[contains(@class, 'parent')]");

                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        var tds = row.SelectNodes("td");
                        if (tds == null || tds.Count < 8) continue;

                        string dataRaw = tds[1].InnerText.Trim();
                        string hora = tds[2].InnerText.Trim();
                        string localCasaFora = tds[3].InnerText.Trim();
                        string adversario = tds[5].InnerText.Trim();
                        string resultado = tds[6].InnerText.Trim();
                        string campeonato = tds[7].InnerText.Trim();

                        campeonato = Regex.Replace(campeonato, @"\r\n?|\n", "").Trim();

                        string dataFormatada = dataRaw;
                        if (DateTime.TryParse(dataRaw, out DateTime dataParse))
                        {
                            dataFormatada = dataParse.ToString("dd/MM/yyyy");
                        }

                        bool americaMandante = localCasaFora.Contains("(C)");
                        string mandante = americaMandante ? "América" : adversario;
                        string visitante = americaMandante ? adversario : "América";
                        string escudoAmericaOgol = "https://cdn-img.staticzz.com/img/logos/equipas/2227_imgbank_1683561378.png";
                        string escudoAdversarioOgol = "https://www.ogol.com.br" + tds[4].SelectSingleNode(".//img")?.GetAttributeValue("src", "");

                        var partidaInfo = new PartidaViewModel
                        {
                            Campeonato = campeonato,
                            Mandante = mandante,
                            EscudoMandante = ObterCaminhoEscudo(mandante),
                            EscudoFallbackMandante = americaMandante ? escudoAmericaOgol : escudoAdversarioOgol,
                            Visitante = visitante,
                            EscudoVisitante = ObterCaminhoEscudo(visitante),
                            EscudoFallbackVisitante = americaMandante ? escudoAdversarioOgol : escudoAmericaOgol,
                            DataHora = $"{dataFormatada} às {hora}"
                        };

                        // Verifica se o resultado tem o formato de placar (ex: 1-4)[cite: 10]
                        var matchPlacar = Regex.Match(resultado, @"(\d+)\s*-\s*(\d+)");

                        if (matchPlacar.Success)
                        {
                            partidaInfo.PlacarMandante = matchPlacar.Groups[1].Value;
                            partidaInfo.PlacarVisitante = matchPlacar.Groups[2].Value;
                            passados.Add(partidaInfo);
                        }
                        else
                        {
                            futuros.Add(partidaInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao extrair jogos sub-20: {ex.Message}");
            }

            // Retorna o passado mais recente (índice 0) e o futuro mais iminente (último da lista)
            return (passados.FirstOrDefault(), futuros.LastOrDefault());
        }

        public List<ClassificacaoViewModel> ObterClassificacaoSub20()
        {
            var classificacao = new List<ClassificacaoViewModel>();

            try
            {
                // URL original da Classificação (Decagonal)
                var document = _web.Load("https://www.ogol.com.br/edition.php?id_edicao=212517&fase=233184");

                // Pega a primeira tabela dentro da div 'edition_table'
                var rows = document.DocumentNode.SelectNodes("(//div[@id='edition_table'])[1]//table/tbody/tr");

                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        var tds = row.SelectNodes("td");
                        if (tds == null || tds.Count < 11) continue;

                        var nomeTime = tds[2].InnerText.Trim();
                        var escudoUrl = "https://www.ogol.com.br" + tds[1].SelectSingleNode(".//img")?.GetAttributeValue("src", "");

                        classificacao.Add(new ClassificacaoViewModel
                        {
                            Posicao = int.TryParse(tds[0].InnerText.Trim(), out int pos) ? pos : 0,
                            Time = new TimeInfo { NomePopular = nomeTime, Escudo = escudoUrl },
                            Pontos = int.TryParse(tds[3].InnerText.Trim(), out int pts) ? pts : 0,
                            Jogos = int.TryParse(tds[4].InnerText.Trim(), out int j) ? j : 0,
                            Vitorias = int.TryParse(tds[5].InnerText.Trim(), out int v) ? v : 0,
                            Empates = int.TryParse(tds[6].InnerText.Trim(), out int e) ? e : 0,
                            Derrotas = int.TryParse(tds[7].InnerText.Trim(), out int d) ? d : 0,
                            GolsPro = int.TryParse(tds[8].InnerText.Trim(), out int gp) ? gp : 0,
                            GolsContra = int.TryParse(tds[9].InnerText.Trim(), out int gc) ? gc : 0,
                            SaldoGols = int.TryParse(tds[10].InnerText.Replace("+", "").Trim(), out int sg) ? sg : 0
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao extrair classificação sub-20: {ex.Message}");
            }

            return classificacao;
        }

        private string ObterCaminhoEscudo(string nomeTime)
        {
            var nomeNormalizado = nomeTime.ToLower()
                .Replace("á", "a").Replace("ã", "a").Replace("é", "e")
                .Replace("í", "i").Replace("ó", "o").Replace("ô", "o")
                .Replace("ç", "c").Trim();

            var mapaTimes = new Dictionary<string, string>
            {
                { "america", "america-mineiro" },
                { "athletic", "athletic" },
                { "atletico", "atletico-mineiro" },
                { "cruzeiro", "cruzeiro" },
                { "coimbra", "coimbra" },
                { "itabirito", "itabirito" },
                { "nacional", "nacional-muriae" },
                { "betim", "betim" },
                { "boston", "boston-city" },
                { "sao joao", "sao-joao-del-rei" }
            };

            var chaveEncontrada = mapaTimes.Keys.FirstOrDefault(k => nomeNormalizado.Contains(k)) ?? "america-mineiro";
            string nomeArquivo = mapaTimes[chaveEncontrada];

            return $"/img/brazil_{nomeArquivo}_700x700.football-logos.cc.png";
        }
    }
}
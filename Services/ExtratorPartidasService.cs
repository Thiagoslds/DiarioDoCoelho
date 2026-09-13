using DiarioDoCoelho.ViewModels;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace DiarioDoCoelho.Services
{
    public class ExtratorPartidasService
    {
        private readonly HtmlWeb _web;

        public ExtratorPartidasService()
        {
            _web = new HtmlWeb();
        }

        // Retorna o anterior, o próximo, e uma lista com 3 jogos para a classificação
        // Modifique a assinatura do método para retornar 4 elementos na Tupla
        public (PartidaViewModel? Anterior, PartidaViewModel? Proximo, List<PartidaViewModel> ProximosJogos, List<PartidaViewModel> JogosAnteriores) ObterJogosAmerica()
        {
            var passados = new List<PartidaViewModel>();
            var futuros = new List<PartidaViewModel>();

            try
            {
                var document = _web.Load("https://www.ogol.com.br/equipe/america-mineiro");
                var rows = document.DocumentNode.SelectNodes("//table[contains(@class, 'zztable stats')]/tbody/tr[contains(@class, 'parent')]");

                if (rows != null)
                {
                    foreach (var row in rows)
                    {
                        var tds = row.SelectNodes("td");
                        if (tds == null || tds.Count < 9) continue;

                        string dataUrl = tds[1].SelectSingleNode(".//a")?.GetAttributeValue("href", "") ?? "";
                        string dataRaw = tds[1].InnerText.Trim(); // <-- AQUI ESTÁ A CORREÇÃO!
                        string hora = tds[2].InnerText.Trim();
                        string campeonato = tds[3].InnerText.Trim();
                        string mandante = tds[4].InnerText.Trim();
                        string resultado = tds[6].InnerText.Trim();
                        string visitante = tds[8].InnerText.Trim();

                        campeonato = Regex.Replace(campeonato, @"\r\n?|\n", "").Trim();

                        // Usa a data raw (ex: "28/09") como plano B caso a URL falhe
                        string dataFormatada = dataRaw;

                        var matchData = Regex.Match(dataUrl, @"\/jogo\/(\d{4})-(\d{2})-(\d{2})");
                        if (matchData.Success)
                        {
                            dataFormatada = $"{matchData.Groups[3].Value}/{matchData.Groups[2].Value}/{matchData.Groups[1].Value}";
                        }

                        // SUBSTITUIÇÃO DO NOME DO AMÉRICA
                        mandante = mandante.Replace("América Mineiro", "América");
                        visitante = visitante.Replace("América Mineiro", "América");

                        string escudoMandanteOgol = "https://www.ogol.com.br" + tds[5].SelectSingleNode(".//img")?.GetAttributeValue("src", "");
                        string escudoVisitanteOgol = "https://www.ogol.com.br" + tds[7].SelectSingleNode(".//img")?.GetAttributeValue("src", "");

                        var partidaInfo = new PartidaViewModel
                        {
                            Campeonato = campeonato,
                            Mandante = mandante,
                            EscudoMandante = ObterCaminhoEscudo(mandante),
                            EscudoFallbackMandante = string.IsNullOrEmpty(escudoMandanteOgol) ? GerarFallbackAvatar(mandante) : escudoMandanteOgol,
                            Visitante = visitante,
                            EscudoVisitante = ObterCaminhoEscudo(visitante),
                            EscudoFallbackVisitante = string.IsNullOrEmpty(escudoVisitanteOgol) ? GerarFallbackAvatar(visitante) : escudoVisitanteOgol,
                            DataHora = $"{dataFormatada} às {hora}"
                        };

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
                Console.WriteLine($"Erro ao extrair partidas do América: {ex.Message}");
            }

            futuros.Reverse();

            var proximo = futuros.FirstOrDefault();
            var anterior = passados.FirstOrDefault();

            // PEGA OS 3 PRÓXIMOS E OS 3 ÚLTIMOS
            var lista3Proximos = futuros.Take(3).ToList();
            var lista3Anteriores = passados.Take(3).ToList();

            return (anterior, proximo, lista3Proximos, lista3Anteriores);
        }

        private string ObterCaminhoEscudo(string nomeTime)
        {
            var nomeNormalizado = nomeTime.ToLower()
                .Replace("á", "a").Replace("ã", "a").Replace("é", "e")
                .Replace("í", "i").Replace("ó", "o").Replace("ô", "o")
                .Replace("ç", "c").Trim();

            if (nomeNormalizado.Contains("america")) return "/img/afc-escudos-site-branco-1.png";
            if (nomeNormalizado.Contains("cruzeiro")) return "/img/cruzeiro_1.png";

            var mapaTimes = new Dictionary<string, string>
            {
                { "athletic", "athletic" },
                { "atletico", "atletico-mineiro" },
                { "avai", "avai" },
                { "botafogo", "botafogo-sp" },
                { "ceara", "ceara" },
                { "crb", "crb" },
                { "criciuma", "criciuma" },
                { "cuiaba", "cuiaba" },
                { "fortaleza", "fortaleza" },
                { "goias", "goias" },
                { "juventude", "juventude" },
                { "londrina", "londrina" },
                { "nautico", "nautico" },
                { "novorizontino", "novorizontino" },
                { "operario", "operario-ferroviario" },
                { "ponte preta", "ponte-preta" },
                { "sao bernardo", "sao-bernardo" },
                { "sport", "sport-recife" },
                { "vila nova", "vila-nova" }
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
            if (nomeTime.Contains("América")) return "/img/afc-escudos-site-branco-1.png";
            return $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(nomeTime)}&background=e6f2e6&color=009e4f&rounded=true&bold=true";
        }
    }
}
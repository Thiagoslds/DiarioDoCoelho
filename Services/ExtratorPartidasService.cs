using HtmlAgilityPack;
using DiarioDoCoelho.ViewModels;
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

        public PartidaViewModel? ObterPartidaAnteriorCBF()
        {
            try
            {
                var document = _web.Load("https://www.cbf.com.br/futebol-brasileiro/times/campeonato-brasileiro/serie-b/2026/59897");
                string htmlCru = document.Text;

                var regex = new Regex(@"\\""mandante\\"":\{.*?\\""nome\\"":\\""([^""]+)\\"".*?\\""gols\\"":\\""([^""]+)\\"".*?\\""visitante\\"":\{.*?\\""nome\\"":\\""([^""]+)\\"".*?\\""gols\\"":\\""([^""]+)\\"".*?\\""campeonato\\"":\\""([^""]+)\\"".*?\\""data\\"":\\""\s*([^""]+)\\"".*?\\""hora\\"":\\""([^""]+)\\""");

                var match = regex.Match(htmlCru);

                if (match.Success)
                {
                    string mandante = match.Groups[1].Value;
                    string visitante = match.Groups[3].Value;

                    return new PartidaViewModel
                    {
                        Mandante = mandante,
                        EscudoMandante = ObterCaminhoEscudo(mandante),
                        PlacarMandante = match.Groups[2].Value,
                        Visitante = visitante,
                        EscudoVisitante = ObterCaminhoEscudo(visitante),
                        PlacarVisitante = match.Groups[4].Value,
                        Campeonato = match.Groups[5].Value,
                        DataHora = $"{match.Groups[6].Value} às {match.Groups[7].Value}"
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao extrair partida anterior: {ex.Message}");
                return null;
            }
        }

        public PartidaViewModel? ObterProximaPartidaClube()
        {
            try
            {
                // Mantenha a URL do site da ESPN[cite: 13]
                var document = _web.Load("https://www.espn.com.br/futebol/time/calendario/_/id/6154/america-mineiro");

                var node = document.DocumentNode.SelectSingleNode("(//tbody[contains(@class, 'Table__TBODY')]/tr)[1]");
                if (node == null) return null;

                var tds = node.SelectNodes("td");
                if (tds == null || tds.Count < 6) return null;

                string dataTexto = tds[0].InnerText.Trim();
                string horaTexto = tds[4].InnerText.Trim();
                string mandante = tds[1].InnerText.Trim();
                string visitante = tds[3].InnerText.Trim();

                return new PartidaViewModel
                {
                    Mandante = mandante,
                    EscudoMandante = ObterCaminhoEscudo(mandante),
                    Visitante = visitante,
                    EscudoVisitante = ObterCaminhoEscudo(visitante),
                    Campeonato = tds[5].InnerText.Trim(),
                    DataHora = $"{dataTexto} às {horaTexto}"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao extrair próxima partida: {ex.Message}");
                return null;
            }
        }

        private string ObterCaminhoEscudo(string nomeTime)
        {
            // Remove acentos e padroniza para minúsculo
            var nomeNormalizado = nomeTime.ToLower()
                .Replace("á", "a").Replace("ã", "a").Replace("é", "e")
                .Replace("í", "i").Replace("ó", "o").Replace("ô", "o")
                .Replace("ç", "c").Trim();

            // Mapeamento dos nomes vindos da raspagem para o nome exato do arquivo local
            var mapaTimes = new Dictionary<string, string>
            {
                { "america", "america-mineiro" },
                { "athletic", "athletic" },
                { "atletico goianiense", "atletico-goianiense" },
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

            // Identifica qual chave do mapa está contida no nome do time raspado
            var chaveEncontrada = mapaTimes.Keys.FirstOrDefault(k => nomeNormalizado.Contains(k)) ?? "america-mineiro";
            string nomeArquivo = mapaTimes[chaveEncontrada];

            // Retorna o caminho apontando para a pasta wwwroot/img (A extensão padrão .png foi adicionada. Ajuste para .jpg se necessário)
            return $"/img/brazil_{nomeArquivo}_700x700.football-logos.cc.png";
        }
    }
}
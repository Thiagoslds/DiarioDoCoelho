using DiarioDoCoelho.ViewModels;

namespace DiarioDoCoelho.Services
{
    public class ExtratorPartidasService
    {
        public (PartidaViewModel? Anterior, PartidaViewModel? Proximo, List<PartidaViewModel> ProximosJogos, List<PartidaViewModel> JogosAnteriores) ObterJogosAmerica()
        {
            string campeonato = "Campeonato Brasileiro Série B";

            // Repositório Estático baseado no PDF oficial
            // Para atualizar o site, basta adicionar o número do placar dentro das aspas ""
            var todosOsJogos = new List<PartidaViewModel>
            {
                // JOGOS PASSADOS
                new PartidaViewModel { Campeonato = campeonato, DataHora = "05/09/2026 às 21:30", Mandante = "CRB", Visitante = "América", PlacarMandante = "3", PlacarVisitante = "1" },
                new PartidaViewModel { Campeonato = campeonato, DataHora = "09/09/2026 às 20:30", Mandante = "América", Visitante = "Náutico", PlacarMandante = "2", PlacarVisitante = "1" },
                new PartidaViewModel { Campeonato = campeonato, DataHora = "14/09/2026 às 19:30", Mandante = "América", Visitante = "São Bernardo FC", PlacarMandante = "0", PlacarVisitante = "2" },

                // JOGOS FUTUROS (Placar em branco)
                new PartidaViewModel { Campeonato = campeonato, DataHora = "18/09/2026 às 19:30", Mandante = "Vila Nova", Visitante = "América", PlacarMandante = "", PlacarVisitante = "" },
                new PartidaViewModel { Campeonato = campeonato, DataHora = "28/09/2026 às 19:30", Mandante = "América", Visitante = "Juventude", PlacarMandante = "", PlacarVisitante = "" },
                new PartidaViewModel { Campeonato = campeonato, DataHora = "03/10/2026 às 16:00", Mandante = "Atlético-GO", Visitante = "América", PlacarMandante = "", PlacarVisitante = "" },
                new PartidaViewModel { Campeonato = campeonato, DataHora = "07/10/2026 às 20:30", Mandante = "América", Visitante = "Fortaleza", PlacarMandante = "", PlacarVisitante = "" }
            };

            // Processamento automático de Escudos
            foreach (var jogo in todosOsJogos)
            {
                jogo.EscudoMandante = ObterCaminhoEscudo(jogo.Mandante);
                jogo.EscudoFallbackMandante = GerarFallbackAvatar(jogo.Mandante);

                jogo.EscudoVisitante = ObterCaminhoEscudo(jogo.Visitante);
                jogo.EscudoFallbackVisitante = GerarFallbackAvatar(jogo.Visitante);
            }

            // A Mágica: Separa sozinho o que é passado (tem placar) do que é futuro (não tem)
            var passados = todosOsJogos.Where(j => !string.IsNullOrEmpty(j.PlacarMandante)).ToList();
            var futuros = todosOsJogos.Where(j => string.IsNullOrEmpty(j.PlacarMandante)).ToList();

            // O jogo anterior é o último da lista de passados
            var anterior = passados.LastOrDefault();

            // O próximo jogo é o primeiro da lista de futuros
            var proximo = futuros.FirstOrDefault();

            // Pega os 3 jogos para a tela de Tabelas
            var proximosJogos = futuros.Take(3).ToList();

            // Inverte a lista de passados para mostrar do mais recente pro mais antigo (14/09 -> 09/09 -> 05/09)
            var jogosAnteriores = passados.AsEnumerable().Reverse().Take(3).ToList();

            return (anterior, proximo, proximosJogos, jogosAnteriores);
        }

        private string ObterCaminhoEscudo(string nomeTime)
        {
            var nomeNormalizado = nomeTime.ToLower()
                .Replace("á", "a").Replace("ã", "a").Replace("é", "e")
                .Replace("í", "i").Replace("ó", "o").Replace("ô", "o")
                .Replace("ç", "c").Trim();

            if (nomeNormalizado.Contains("america")) return "/img/afc-escudos-site-branco-1.png";

            var mapaTimes = new Dictionary<string, string>
            {
                { "athletic", "athletic" },
                { "atletico-go", "atletico-goianiense" },
                { "atletico goianiense", "atletico-goianiense" },
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
            if (nomeTime == "América") return "/img/afc-escudos-site-branco-1.png";
            return $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(nomeTime)}&background=e6f2e6&color=009e4f&rounded=true&bold=true";
        }
    }
}
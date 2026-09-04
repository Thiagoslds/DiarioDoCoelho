using System.Text.Json.Serialization;

namespace DiarioDoCoelho.ViewModels
{
    public class ClassificacaoViewModel
    {
        [JsonPropertyName("posicao")]
        public int Posicao { get; set; }

        [JsonPropertyName("pontos")]
        public int Pontos { get; set; }

        [JsonPropertyName("time")]
        public TimeInfo Time { get; set; }

        [JsonPropertyName("jogos")]
        public int Jogos { get; set; }

        [JsonPropertyName("vitorias")]
        public int Vitorias { get; set; }

        [JsonPropertyName("empates")]
        public int Empates { get; set; }

        [JsonPropertyName("derrotas")]
        public int Derrotas { get; set; }

        [JsonPropertyName("gols_pro")]
        public int GolsPro { get; set; }

        [JsonPropertyName("gols_contra")]
        public int GolsContra { get; set; }

        [JsonPropertyName("saldo_gols")]
        public int SaldoGols { get; set; }

        [JsonPropertyName("aproveitamento")]
        public double Aproveitamento { get; set; }

        [JsonPropertyName("ultimos_jogos")]
        public List<string> UltimosJogos { get; set; }
    }

    public class TimeInfo
    {
        [JsonPropertyName("time_id")]
        public int TimeId { get; set; }

        [JsonPropertyName("nome_popular")]
        public string NomePopular { get; set; }

        [JsonPropertyName("sigla")]
        public string Sigla { get; set; }

        [JsonPropertyName("escudo")]
        public string Escudo { get; set; }
    }
}
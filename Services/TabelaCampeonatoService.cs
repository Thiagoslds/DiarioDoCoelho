using DiarioDoCoelho.ViewModels;
using System.Text.Json;
using System.Net.Http.Headers;

namespace DiarioDoCoelho.Services
{
    public class TabelaCampeonatoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TabelaCampeonatoService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            var baseUrl = _configuration["ApiFutebol:BaseUrl"];
            var apiKey = _configuration["ApiFutebol:ApiKey"];

            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<List<ClassificacaoViewModel>> ObterTabelaAsync(int campeonatoId)
        {
            var response = await _httpClient.GetAsync($"campeonatos/{campeonatoId}/tabela");

            if (!response.IsSuccessStatusCode)
            {
                // Em produção, adicione logs estruturados aqui
                return new List<ClassificacaoViewModel>();
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ClassificacaoViewModel>>(content) ?? new List<ClassificacaoViewModel>();
        }
    }
}
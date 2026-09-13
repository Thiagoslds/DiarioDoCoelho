using DiarioDoCoelho.Models;

namespace DiarioDoCoelho.ViewModels
{
    public class HomeIndexViewModel
    {
        public Post? PostDestaque { get; set; }
        public List<Post> UltimosPosts { get; set; } = new();
        public PartidaViewModel? PartidaAnterior { get; set; }
        public PartidaViewModel? ProximaPartida { get; set; }
        public List<BannerAfiliado> BannersAtivos { get; set; } = new();
        public List<Post> GiroNoticias { get; set; } = new();
        public List<Post> Artigos { get; set; } = new();
        public List<ProdutoLoja> ProdutosLoja { get; set; } = new();
        public List<NoticiaCoelho> NoticiasExternas { get; set; } = new List<NoticiaCoelho>();
    }
}

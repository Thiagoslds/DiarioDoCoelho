using DiarioDoCoelho.Models;

namespace DiarioDoCoelho.ViewModels
{
    public class HomeIndexViewModel
    {
        public Post? PostDestaque { get; set; }
        public List<Post> UltimosPosts { get; set; } = new();
        public List<Jogo> ProximosJogos { get; set; } = new();
        public List<BannerAfiliado> BannersAtivos { get; set; } = new();
        public List<Post> GiroNoticias { get; set; } = new();
        public List<Post> Artigos { get; set; } = new();
        public List<Post> BauDoCoelho { get; set; } = new();
        public List<ProdutoAfiliadoViewModel> ProdutosLoja { get; set; } = new();
        public List<NoticiaCoelho> NoticiasExternas { get; set; } = new List<NoticiaCoelho>();
    }
}

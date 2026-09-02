using DiarioDoCoelho.Models;

namespace DiarioDoCoelho.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Post> UltimosPosts { get; set; } = new();
        public Jogo? ProximoJogo { get; set; }
        public List<BannerAfiliado> BannersAtivos { get; set; } = new();
    }
}
